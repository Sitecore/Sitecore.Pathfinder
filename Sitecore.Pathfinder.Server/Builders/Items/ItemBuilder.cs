namespace Sitecore.Pathfinder.Builders.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.IO;

  public class ItemBuilder
  {
    public ItemBuilder([NotNull] Projects.Items.Item projectItem)
    {
      this.ProjectItem = projectItem;
    }

    [CanBeNull]
    public Item Item { get; protected set; }

    [NotNull]
    public Projects.Items.Item ProjectItem { get; }

    [CanBeNull]
    protected Item TemplateItem { get; set; }

    public void Build([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ProjectItem.DatabaseName);
      if (database == null)
      {
        throw new EmitException(Texts.Database_not_found, this.ProjectItem.Snapshot, this.ProjectItem.DatabaseName);
      }

      if (this.Item == null)
      {
        this.ResolveItem(context);
      }

      if (this.TemplateItem == null)
      {
        this.ResolveTemplateItem(context);
      }

      if (this.TemplateItem == null && this.Item != null)
      {
        this.TemplateItem = this.Item.Template;
      }

      if (this.Item == null)
      {
        this.CreateNewItem(context, database);
      }

      this.Validate();

      this.UpdateFields(context);
      this.UpdateItem(context);
    }

    protected void CreateNewItem([NotNull] IEmitContext context, [NotNull] Database database)
    {
      if (this.TemplateItem == null)
      {
        throw new RetryableEmitException(Texts.Template_missing, this.ProjectItem.Snapshot, this.ProjectItem.TemplateIdOrPath);
      }

      var parentItemPath = PathHelper.GetItemParentPath(this.ProjectItem.ItemIdOrPath);

      var parentItem = database.CreateItemPath(parentItemPath);
      if (parentItem == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_item_path, this.ProjectItem.Snapshot, parentItemPath);
      }

      // item is created with correct ID
      var item = ItemManager.AddFromTemplate(this.ProjectItem.ItemName, this.TemplateItem.ID, parentItem, new ID(this.ProjectItem.Guid));
      if (item == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_item_path, this.ProjectItem.Snapshot, this.ProjectItem.ItemIdOrPath);
      }

      this.Item = item;
      this.ProjectItem.ItemIdOrPath = item.ID.ToString();
    }

    protected virtual void ResolveItem([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ProjectItem.DatabaseName);
      if (database == null)
      {
        return;
      }

      this.Item = database.GetItem(new ID(this.ProjectItem.Guid));
      if (this.Item != null)
      {
        this.ProjectItem.ItemIdOrPath = this.Item.ID.ToString();
      }
    }

    protected void ResolveTemplateItem([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ProjectItem.DatabaseName);
      if (database == null)
      {
        return;
      }

      if (ID.IsID(this.ProjectItem.TemplateIdOrPath))
      {
        this.TemplateItem = database.GetItem(this.ProjectItem.TemplateIdOrPath);
      }

      if (this.TemplateItem == null && this.ProjectItem.TemplateIdOrPath.Contains("/"))
      {
        this.TemplateItem = database.GetItem(this.ProjectItem.TemplateIdOrPath);
      }

      if (this.TemplateItem != null)
      {
        this.ProjectItem.TemplateIdOrPath = this.TemplateItem.ID.ToString();
      }
    }

    protected void UpdateFields([NotNull] IEmitContext context)
    {
      if (this.Item == null)
      {
        throw new EmitException(Texts.Item_not_found, this.ProjectItem.Snapshot);
      }

      foreach (var field in this.ProjectItem.Fields)
      {
        foreach (var fieldResolver in context.FieldResolvers)
        {
          if (fieldResolver.CanResolve(context, field, this.Item))
          {
            fieldResolver.Resolve(context, field, this.Item);
          }
        }
      }
    }

    protected void UpdateItem([NotNull] IEmitContext context)
    {
      if (this.Item == null)
      {
        throw new EmitException(Texts.Item_not_found, this.ProjectItem.Snapshot, this.ProjectItem.ItemIdOrPath);
      }

      if (this.TemplateItem == null)
      {
        throw new RetryableEmitException(Texts.Template_missing, this.ProjectItem.Snapshot, this.ProjectItem.TemplateIdOrPath);
      }

      // move
      if (string.Compare(this.Item.Paths.Path, this.ProjectItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(this.Item.ID.ToString(), this.ProjectItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0)
      {
        var parentItemPath = PathHelper.GetItemParentPath(this.ProjectItem.ItemIdOrPath);

        var parentItem = this.Item.Database.GetItem(parentItemPath);
        if (parentItem == null)
        {
          parentItem = this.Item.Database.CreateItemPath(parentItemPath);
          if (parentItem == null)
          {
            throw new RetryableEmitException(Texts.Could_not_create_item, this.ProjectItem.Snapshot, parentItemPath);
          }
        }

        this.Item.MoveTo(parentItem);
      }

      // rename and update fields
      using (new EditContext(this.Item))
      {
        if (this.Item.Name != this.ProjectItem.ItemName)
        {
          this.Item.Name = this.ProjectItem.ItemName;
        }

        if (this.Item.TemplateID != this.TemplateItem.ID)
        {
          var templateItem = new TemplateItem(this.TemplateItem);
          this.Item.ChangeTemplate(templateItem);
        }

        foreach (var field in this.ProjectItem.Fields.Where(i => string.IsNullOrEmpty(i.Language) && i.Version == 0))
        {
          this.Item[field.Name] = field.Value;
        }
      }

      var items = new List<Item>();

      foreach (var field in this.ProjectItem.Fields.Where(i => !string.IsNullOrEmpty(i.Language) || i.Version != 0))
      {
        // language has already been validated
        var language = LanguageManager.GetLanguage(field.Language, this.Item.Database);

        var item = items.FirstOrDefault(i => i.Language == language && i.Version.Number == field.Version);
        if (item == null)
        {
          item = this.Item.Database.GetItem(this.Item.ID, language, new Sitecore.Data.Version(field.Version));
          if (item == null)
          {
            // todo: validate this before updating the item
            context.Trace.TraceError(Texts.Item_not_found, field.TextNode.Snapshot.SourceFile.FileName, field.TextNode.Position, $"language: {field.Language}, version; {field.Version}");
            continue;
          }

          item.Editing.BeginEdit();
          items.Add(item);
        }

        item[field.Name] = field.Value;
      }

      foreach (var item in items)
      {
        item.Editing.EndEdit();
      }
    }

    private void Validate()
    {
      if (this.TemplateItem == null)
      {
        return;
      }

      var template = TemplateManager.GetTemplate(this.TemplateItem.ID, this.TemplateItem.Database);
      if (template == null)
      {
        return;
      }

      var templateFields = template.GetFields(true);

      foreach (var field in this.ProjectItem.Fields)
      {
        if (templateFields.All(f => string.Compare(f.Name, field.Name, StringComparison.OrdinalIgnoreCase) != 0))
        {
          throw new RetryableEmitException(Texts.Field_is_not_defined_in_the_template, this.ProjectItem.Snapshot, field.Name);
        }

        if (!string.IsNullOrEmpty(field.Language))
        {
          var language = LanguageManager.GetLanguage(field.Language, this.TemplateItem.Database);
          if (language == null)
          {
            throw new RetryableEmitException(Texts.Language_not_found, field.TextNode, field.Language);
          }
        }
      }
    }
  }
}
