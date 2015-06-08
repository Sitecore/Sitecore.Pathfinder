namespace Sitecore.Pathfinder.Builders.Items
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.IO;

  public class ItemBuilder
  {
    [NotNull]
    public string DatabaseName { get; set; } = string.Empty;

    [NotNull]
    public ICollection<FieldBuilder> Fields { get; } = new List<FieldBuilder>();

    public Guid Guid { get; set; } = Guid.Empty;

    [NotNull]
    public string ItemIdOrPath { get; set; } = string.Empty;

    [NotNull]
    public string ItemName { get; set; } = string.Empty;

    [NotNull]
    public ISnapshot Snapshot { get; set; }

    [NotNull]
    public string TemplateIdOrPath { get; set; } = string.Empty;

    public void Build([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.DatabaseName);
      if (database == null)
      {
        throw new EmitException(Texts.Database_not_found, this.Snapshot, this.DatabaseName);
      }

      var item = database.GetItem(new ID(this.Guid));
      if (item != null)
      {
        this.ItemIdOrPath = item.Paths.Path;
      }

      var templateItem = database.GetItem(this.TemplateIdOrPath);
      if (templateItem == null && item != null)
      {
        templateItem = item.Template;
      }

      if (templateItem == null)
      {
        throw new RetryableEmitException(Texts.Template_missing, this.Snapshot, this.TemplateIdOrPath);
      }

      if (item == null)
      {
        item = this.CreateNewItem(context, database, templateItem);
        context.RegisterAddedItem(item);
      }
      else
      {
        context.RegisterUpdatedItem(item);
      }

      this.UpdateItem(context, item, templateItem);
    }

    [NotNull]
    protected Item CreateNewItem([NotNull] IEmitContext context, [NotNull] Database database, [NotNull] Item templateItem)
    {
      var parentItemPath = PathHelper.GetItemParentPath(this.ItemIdOrPath);

      var parentItem = database.CreateItemPath(parentItemPath);
      if (parentItem == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_item_path, this.Snapshot, parentItemPath);
      }

      var item = ItemManager.AddFromTemplate(this.ItemName, templateItem.ID, parentItem, new ID(this.Guid));
      if (item == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_item_path, this.Snapshot, this.ItemIdOrPath);
      }

      this.ItemIdOrPath = item.ID.ToString();

      return item;
    }

    protected void UpdateItem([NotNull] IEmitContext context, [NotNull] Item item, [NotNull] Item templateItem)
    {
      // move
      if (string.Compare(item.Paths.Path, this.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(item.ID.ToString(), this.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0)
      {
        var parentItemPath = PathHelper.GetItemParentPath(this.ItemIdOrPath);

        var parentItem = item.Database.GetItem(parentItemPath);
        if (parentItem == null)
        {
          parentItem = item.Database.CreateItemPath(parentItemPath);
          if (parentItem == null)
          {
            throw new RetryableEmitException(Texts.Could_not_create_item, this.Snapshot, parentItemPath);
          }
        }

        item.MoveTo(parentItem);
      }

      // rename and update fields
      using (new EditContext(item))
      {
        if (item.Name != this.ItemName)
        {
          item.Name = this.ItemName;
        }

        if (item.TemplateID != templateItem.ID)
        {
          item.ChangeTemplate(new TemplateItem(templateItem));
        }

        foreach (var field in this.Fields.Where(i => string.IsNullOrEmpty(i.Language) && i.Version == 0))
        {
          item[field.FieldName.Value] = field.Value;
        }
      }

      var versionedItems = new List<Item>();

      foreach (var field in this.Fields.Where(i => !string.IsNullOrEmpty(i.Language) || i.Version != 0))
      {
        // language has already been validated
        var language = LanguageManager.GetLanguage(field.Language, item.Database);

        var versionedItem = versionedItems.FirstOrDefault(i => i.Language == language && i.Version.Number == field.Version);
        if (versionedItem == null)
        {
          versionedItem = item.Database.GetItem(item.ID, language, new Sitecore.Data.Version(field.Version));
          if (versionedItem == null)
          {
            // todo: validate this before updating the item
            context.Trace.TraceError(Texts.Item_not_found, field.FieldName.Source ?? TextNode.Empty, $"language: {field.Language}, version; {field.Version}");
            continue;
          }

          versionedItem.Editing.BeginEdit();
          versionedItems.Add(versionedItem);
        }

        versionedItem[field.FieldName.Value] = field.Value;
      }

      foreach (var i in versionedItems)
      {
        i.Editing.EndEdit();
      }
    }
  }
}
