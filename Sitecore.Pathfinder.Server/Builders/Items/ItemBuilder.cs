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
    [CanBeNull]
    public Item Item { get; protected set; }

    [CanBeNull]
    protected Item TemplateItem { get; set; }

    public void Build([NotNull] IEmitContext context, [NotNull] Projects.Items.Item projectItem)
    {
      var database = context.DataService.GetDatabase(projectItem.DatabaseName);
      if (database == null)
      {
        throw new EmitException(Texts.Database_not_found, projectItem.Snapshot, projectItem.DatabaseName);
      }

      if (this.Item == null)
      {
        this.ResolveItem(context, projectItem);
      }

      if (this.TemplateItem == null)
      {
        this.ResolveTemplateItem(context, projectItem);
      }

      if (this.TemplateItem == null && this.Item != null)
      {
        this.TemplateItem = this.Item.Template;
      }

      this.Validate(projectItem);

      if (this.Item == null)
      {
        this.CreateNewItem(context, database, projectItem);
        if (this.Item == null)
        {
          return;
        }

        context.RegisterAddedItem(this.Item);
      }
      else
      {
        context.RegisterUpdatedItem(this.Item);
      }

      this.UpdateFields(context, projectItem);
      this.UpdateItem(context, projectItem);
    }

    protected void CreateNewItem([NotNull] IEmitContext context, [NotNull] Database database, [NotNull] Projects.Items.Item projectItem)
    {
      if (this.TemplateItem == null)
      {
        throw new RetryableEmitException(Texts.Template_missing, projectItem.Snapshot, projectItem.TemplateIdOrPath);
      }

      var parentItemPath = PathHelper.GetItemParentPath(projectItem.ItemIdOrPath);

      var parentItem = database.CreateItemPath(parentItemPath);
      if (parentItem == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_item_path, projectItem.Snapshot, parentItemPath);
      }

      var item = ItemManager.AddFromTemplate(projectItem.ItemName, this.TemplateItem.ID, parentItem, new ID(projectItem.Guid));
      if (item == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_item_path, projectItem.Snapshot, projectItem.ItemIdOrPath);
      }

      this.Item = item;
      projectItem.ItemIdOrPath = item.ID.ToString();
    }

    protected virtual void ResolveItem([NotNull] IEmitContext context, [NotNull] Projects.Items.Item projectItem)
    {
      var database = context.DataService.GetDatabase(projectItem.DatabaseName);
      if (database == null)
      {
        return;
      }

      this.Item = database.GetItem(new ID(projectItem.Guid));
      if (this.Item != null)
      {
        projectItem.ItemIdOrPath = this.Item.ID.ToString();
      }
    }

    protected void ResolveTemplateItem([NotNull] IEmitContext context, [NotNull] Projects.Items.Item projectItem)
    {
      var database = context.DataService.GetDatabase(projectItem.DatabaseName);
      if (database == null)
      {
        return;
      }

      if (ID.IsID(projectItem.TemplateIdOrPath))
      {
        this.TemplateItem = database.GetItem(projectItem.TemplateIdOrPath);
      }

      if (this.TemplateItem == null && projectItem.TemplateIdOrPath.Contains("/"))
      {
        this.TemplateItem = database.GetItem(projectItem.TemplateIdOrPath);
      }

      if (this.TemplateItem != null)
      {
        projectItem.TemplateIdOrPath = this.TemplateItem.ID.ToString();
      }
    }

    protected void UpdateFields([NotNull] IEmitContext context, [NotNull] Projects.Items.Item projectItem)
    {
      if (this.Item == null)
      {
        throw new EmitException(Texts.Item_not_found, projectItem.Snapshot);
      }

      foreach (var field in projectItem.Fields)
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

    protected void UpdateItem([NotNull] IEmitContext context, [NotNull] Projects.Items.Item projectItem)
    {
      if (this.Item == null)
      {
        throw new EmitException(Texts.Item_not_found, projectItem.Snapshot, projectItem.ItemIdOrPath);
      }

      if (this.TemplateItem == null)
      {
        throw new RetryableEmitException(Texts.Template_missing, projectItem.Snapshot, projectItem.TemplateIdOrPath);
      }

      // move
      if (string.Compare(this.Item.Paths.Path, projectItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(this.Item.ID.ToString(), projectItem.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0)
      {
        var parentItemPath = PathHelper.GetItemParentPath(projectItem.ItemIdOrPath);

        var parentItem = this.Item.Database.GetItem(parentItemPath);
        if (parentItem == null)
        {
          parentItem = this.Item.Database.CreateItemPath(parentItemPath);
          if (parentItem == null)
          {
            throw new RetryableEmitException(Texts.Could_not_create_item, projectItem.Snapshot, parentItemPath);
          }
        }

        this.Item.MoveTo(parentItem);
      }

      // rename and update fields
      using (new EditContext(this.Item))
      {
        if (this.Item.Name != projectItem.ItemName)
        {
          this.Item.Name = projectItem.ItemName;
        }

        if (this.Item.TemplateID != this.TemplateItem.ID)
        {
          var templateItem = new TemplateItem(this.TemplateItem);
          this.Item.ChangeTemplate(templateItem);
        }

        foreach (var field in projectItem.Fields.Where(i => string.IsNullOrEmpty(i.Language) && i.Version == 0))
        {
          this.Item[field.FieldName] = field.Value;
        }
      }

      var items = new List<Item>();

      foreach (var field in projectItem.Fields.Where(i => !string.IsNullOrEmpty(i.Language) || i.Version != 0))
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
            context.Trace.TraceError(Texts.Item_not_found, field.ValueProperty.TextNode.Snapshot.SourceFile.FileName, field.ValueProperty.TextNode.Position, $"language: {field.Language}, version; {field.Version}");
            continue;
          }

          item.Editing.BeginEdit();
          items.Add(item);
        }

        item[field.FieldName] = field.Value;
      }

      foreach (var item in items)
      {
        item.Editing.EndEdit();
      }
    }

    private void Validate([NotNull] Projects.Items.Item projectItem)
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

      foreach (var field in projectItem.Fields)
      {
        if (templateFields.All(f => string.Compare(f.Name, field.FieldName, StringComparison.OrdinalIgnoreCase) != 0))
        {
          throw new RetryableEmitException(Texts.Field_is_not_defined_in_the_template, field.NameProperty.TextNode, field.FieldName);
        }

        if (!string.IsNullOrEmpty(field.Language))
        {
          var language = LanguageManager.GetLanguage(field.Language, this.TemplateItem.Database);
          if (language == null)
          {
            throw new RetryableEmitException(Texts.Language_not_found, field.ValueProperty.TextNode, field.Language);
          }
        }
      }
    }
  }
}
