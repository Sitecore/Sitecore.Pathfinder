namespace Sitecore.Pathfinder.Builders.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Builders.FieldResolvers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Models.Items;

  public class ItemBuilder
  {
    public ItemBuilder([NotNull] ItemModel itemModel)
    {
      this.ItemModel = itemModel;
    }

    [CanBeNull]
    public Item Item { get; set; }

    [NotNull]
    public ItemModel ItemModel { get; }

    [ImportMany]
    [NotNull]
    protected IEnumerable<IFieldResolver> FieldHandlers { get; set; }

    [CanBeNull]
    protected Item TemplateItem { get; set; }

    public void Build([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ItemModel.DatabaseName);
      if (database == null)
      {
        throw new BuildException(Texts.Text2018, this.ItemModel.SourceFileName, 0, 0, this.ItemModel.DatabaseName);
      }

      if (this.TemplateItem == null)
      {
        this.ResolveTemplateItem(context);
      }

      if (this.TemplateItem == null)
      {
        throw new RetryableBuildException(Texts.Text2017, this.ItemModel.SourceFileName, 0, 0, this.ItemModel.TemplateIdOrPath);
      }

      if (this.Item == null)
      {
        this.ResolveItem(context);
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
      if (ID.IsID(this.ItemModel.ItemIdOrPath))
      {
        throw new BuildException(Texts.Text2002, this.ItemModel.SourceFileName);
      }

      if (this.TemplateItem == null)
      {
        throw new BuildException(Texts.Text2016, this.ItemModel.SourceFileName, 0, 0, this.ItemModel.TemplateIdOrPath);
      }

      var item = database.CreateItemPath(this.ItemModel.ItemIdOrPath, new TemplateItem(this.TemplateItem));
      if (item == null)
      {
        throw new RetryableBuildException(Texts.Text2019, this.ItemModel.SourceFileName, 0, 0, this.ItemModel.ItemIdOrPath);
      }

      this.Item = item;
      this.ItemModel.ItemIdOrPath = item.ID.ToString();
    }

    protected virtual void ResolveItem([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ItemModel.DatabaseName);
      if (database == null)
      {
        return;
      }

      if (ID.IsID(this.ItemModel.ItemIdOrPath))
      {
        this.Item = database.GetItem(this.ItemModel.ItemIdOrPath);
      }
      else if (this.ItemModel.ItemIdOrPath.Contains("/"))
      {
        this.Item = database.GetItem(this.ItemModel.ItemIdOrPath);
      }

      if (this.Item != null)
      {
        this.ItemModel.ItemIdOrPath = this.Item.ID.ToString();
      }
    }

    protected void ResolveTemplateItem([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ItemModel.DatabaseName);
      if (database == null)
      {
        return;
      }

      if (ID.IsID(this.ItemModel.TemplateIdOrPath))
      {
        this.TemplateItem = database.GetItem(this.ItemModel.TemplateIdOrPath);
      }

      if (this.TemplateItem == null && this.ItemModel.TemplateIdOrPath.Contains("/"))
      {
        this.TemplateItem = database.GetItem(this.ItemModel.TemplateIdOrPath);
      }

      if (this.TemplateItem != null)
      {
        this.ItemModel.TemplateIdOrPath = this.TemplateItem.ID.ToString();
      }
    }

    protected void UpdateFields([NotNull] IEmitContext context)
    {
      if (this.Item == null)
      {
        throw new BuildException(Texts.Text2003, this.ItemModel.SourceFileName);
      }

      foreach (var field in this.ItemModel.Fields)
      {
        foreach (var fieldHandler in context.FieldHandlers)
        {
          if (!fieldHandler.CanHandle(context, field, this.Item))
          {
            continue;
          }

          fieldHandler.Handle(context, field, this.Item);
        }
      }
    }

    protected void UpdateItem([NotNull] IEmitContext context)
    {
      if (this.Item == null)
      {
        throw new BuildException(Texts.Text2003, this.ItemModel.SourceFileName);
      }

      if (this.TemplateItem == null)
      {
        throw new BuildException(Texts.Text2017, this.ItemModel.SourceFileName);
      }

      using (new EditContext(this.Item))
      {
        if (this.Item.TemplateID != this.TemplateItem.ID)
        {
          var templateItem = new TemplateItem(this.TemplateItem);
          this.Item.ChangeTemplate(templateItem);
        }

        foreach (var field in this.ItemModel.Fields)
        {
          this.Item[field.Name] = field.Value;
        }
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

      foreach (var field in this.ItemModel.Fields)
      {
        if (templateFields.All(f => string.Compare(f.Name, field.Name, StringComparison.OrdinalIgnoreCase) != 0))
        {
          throw new RetryableBuildException(Texts.Text2035, this.ItemModel.SourceFileName, field.SourceElement);
        }
      }
    }
  }
}
