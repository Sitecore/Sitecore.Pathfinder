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
  using Sitecore.Pathfinder.Projects.Items;

  public class ItemBuilder
  {
    public ItemBuilder([NotNull] Projects.Items.Item projectItem)
    {
      this.ProjectItem = projectItem;
    }

    [CanBeNull]
    public Sitecore.Data.Items.Item Item { get; set; }

    [NotNull]
    public Projects.Items.Item ProjectItem { get; }

    [ImportMany]
    [NotNull]
    protected IEnumerable<IFieldResolver> FieldHandlers { get; set; }

    [CanBeNull]
    protected Sitecore.Data.Items.Item TemplateItem { get; set; }

    public void Build([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.ProjectItem.DatabaseName);
      if (database == null)
      {
        throw new BuildException(Texts.Text2018, this.ProjectItem.Location.SourceFileName, 0, 0, this.ProjectItem.DatabaseName);
      }

      if (this.TemplateItem == null)
      {
        this.ResolveTemplateItem(context);
      }

      if (this.TemplateItem == null)
      {
        throw new RetryableBuildException(Texts.Text2017, this.ProjectItem.Location.SourceFileName, 0, 0, this.ProjectItem.TemplateIdOrPath);
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
      if (ID.IsID(this.ProjectItem.ItemIdOrPath))
      {
        throw new BuildException(Texts.Text2002, this.ProjectItem.Location.SourceFileName);
      }

      if (this.TemplateItem == null)
      {
        throw new BuildException(Texts.Text2016, this.ProjectItem.Location.SourceFileName, 0, 0, this.ProjectItem.TemplateIdOrPath);
      }

      var item = database.CreateItemPath(this.ProjectItem.ItemIdOrPath, new TemplateItem(this.TemplateItem));
      if (item == null)
      {
        throw new RetryableBuildException(Texts.Text2019, this.ProjectItem.Location.SourceFileName, 0, 0, this.ProjectItem.ItemIdOrPath);
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

      if (ID.IsID(this.ProjectItem.ItemIdOrPath))
      {
        this.Item = database.GetItem(this.ProjectItem.ItemIdOrPath);
      }
      else if (this.ProjectItem.ItemIdOrPath.Contains("/"))
      {
        this.Item = database.GetItem(this.ProjectItem.ItemIdOrPath);
      }

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
        throw new BuildException(Texts.Text2003, this.ProjectItem.Location.SourceFileName);
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
        throw new BuildException(Texts.Text2003, this.ProjectItem.Location.SourceFileName);
      }

      if (this.TemplateItem == null)
      {
        throw new BuildException(Texts.Text2017, this.ProjectItem.Location.SourceFileName);
      }

      using (new EditContext(this.Item))
      {
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
          throw new RetryableBuildException(Texts.Text2035, this.ProjectItem.Location.SourceFileName, field.SourceElement);
        }
      }
    }
  }
}
