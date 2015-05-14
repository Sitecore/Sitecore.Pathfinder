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
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;

  public class ItemBuilder
  {
    public ItemBuilder([NotNull] Projects.Items.Item projectItem)
    {
      this.ProjectItem = projectItem;
    }

    [CanBeNull]
    public Item Item { get; set; }

    [NotNull]
    public Projects.Items.Item ProjectItem { get; }

    [ImportMany]
    [NotNull]
    protected IEnumerable<IFieldResolver> FieldHandlers { get; set; }

    [CanBeNull]
    protected Item TemplateItem { get; set; }

    public void Build([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ProjectItem.DatabaseName);
      if (database == null)
      {
        throw new BuildException(Texts.Text2018, this.ProjectItem.TextNode, this.ProjectItem.DatabaseName);
      }

      if (this.TemplateItem == null)
      {
        this.ResolveTemplateItem(context);
      }

      if (this.TemplateItem == null)
      {
        throw new RetryableBuildException(Texts.Text2017, this.ProjectItem.TextNode, this.ProjectItem.TemplateIdOrPath);
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
      if (this.TemplateItem == null)
      {
        throw new BuildException(Texts.Text2016, this.ProjectItem.TextNode, this.ProjectItem.TemplateIdOrPath);
      }

      var parentItemPath = PathHelper.GetItemParentPath(this.ProjectItem.ItemIdOrPath);

      var parentItem = database.CreateItemPath(parentItemPath);
      if (parentItem == null)
      {
        throw new RetryableBuildException(Texts.Text2019, this.ProjectItem.TextNode, parentItemPath);
      }

      // item is created with correct ID
      var item = ItemManager.AddFromTemplate(this.ProjectItem.ItemName, this.TemplateItem.ID, parentItem, new ID(this.ProjectItem.Guid));
      if (item == null)
      {
        throw new RetryableBuildException(Texts.Text2019, this.ProjectItem.TextNode, this.ProjectItem.ItemIdOrPath);
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
        throw new BuildException(Texts.Text2003, this.ProjectItem.TextNode);
      }

      foreach (var field in this.ProjectItem.Fields)
      {
        foreach (var fieldHandler in context.FieldResolvers)
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
        throw new BuildException(Texts.Text2003, this.ProjectItem.TextNode);
      }

      if (this.TemplateItem == null)
      {
        throw new BuildException(Texts.Text2017, this.ProjectItem.TextNode);
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
            throw new RetryableBuildException(Texts.Text3028, this.ProjectItem.TextNode, parentItemPath);
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

        foreach (var field in this.ProjectItem.Fields)
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

      foreach (var field in this.ProjectItem.Fields)
      {
        if (templateFields.All(f => string.Compare(f.Name, field.Name, StringComparison.OrdinalIgnoreCase) != 0))
        {
          throw new RetryableBuildException(Texts.Text2035, this.ProjectItem.TextNode);
        }
      }
    }
  }
}