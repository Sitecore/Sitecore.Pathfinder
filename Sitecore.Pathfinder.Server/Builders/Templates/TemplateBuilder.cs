namespace Sitecore.Pathfinder.Builders.Templates
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Templates;

  public class TemplateBuilder
  {
    private static readonly char[] Pipe = { '|' };

    [CanBeNull]
    private IEnumerable<TemplateSectionBuilder> sectionBuilders;

    public TemplateBuilder([NotNull] TemplateModel templateModel)
    {
      this.TemplateModel = templateModel;
    }

    [CanBeNull]
    public Item Item { get; set; }

    [NotNull]
    public IEnumerable<TemplateSectionBuilder> Sections
    {
      get
      {
        return this.sectionBuilders ?? (this.sectionBuilders = this.TemplateModel.Sections.Select(s => new TemplateSectionBuilder(s)).ToList());
      }
    }

    [NotNull]
    public TemplateModel TemplateModel { get; }

    [CanBeNull]
    public Item Build([NotNull] IEmitContext context)
    {
      var inheritedFields = this.GetInheritedFields(this.TemplateModel);

      if (this.Item == null)
      {
        this.ResolveItem(context);
      }

      if (this.Item == null)
      {
        this.CreateNewTemplate(context, inheritedFields);
      }
      else
      {
        this.UpdateTemplate(inheritedFields);
        this.DeleteSections();
      }

      this.SortSections(this);

      return this.Item;
    }

    public void CreateStandardValuesItem([NotNull] IEmitContext context)
    {
      var item = this.Item;
      if (item == null)
      {
        throw new BuildException(Texts.Text2036);
      }

      var standardValuesItem = item.Add("__Standard Values", new TemplateID(item.TemplateID));
      if (standardValuesItem == null)
      {
        throw new BuildException(Texts.Text2023, this.TemplateModel.SourceFileName);
      }

      using (new EditContext(item))
      {
        item[FieldIDs.StandardValueHolderId] = standardValuesItem.ID.ToString();
      }
    }

    protected virtual void CreateNewTemplate([NotNull] IEmitContext context, [NotNull] IEnumerable<TemplateField> inheritedFields)
    {
      var database = context.DataService.GetDatabase(this.TemplateModel.DatabaseName);
      if (database == null)
      {
        return;
      }

      var parentItem = this.GetParentItem(context, database);
      if (parentItem == null)
      {
        throw new BuildException(Texts.Text2004, this.TemplateModel.SourceFileName);
      }

      if (string.IsNullOrEmpty(this.TemplateModel.ItemIdOrPath))
      {
        this.TemplateModel.ItemIdOrPath = Guid.NewGuid().ToString("B").ToUpperInvariant();
      }

      Item item;
      if (ID.IsID(this.TemplateModel.ItemIdOrPath))
      {
        item = ItemManager.AddFromTemplate(this.TemplateModel.Name, new TemplateID(TemplateIDs.Template), parentItem, new ID(this.TemplateModel.ItemIdOrPath));
      }
      else
      {
        item = ItemManager.AddFromTemplate(this.TemplateModel.Name, new TemplateID(TemplateIDs.Template), parentItem);
      }

      if (item == null)
      {
        throw new BuildException(Texts.Text2023, this.TemplateModel.SourceFileName);
      }

      this.Item = item;
      using (new EditContext(item))
      {
        item[FieldIDs.BaseTemplate] = this.TemplateModel.BaseTemplates;
        item.Appearance.Icon = this.TemplateModel.Icon;
      }

      foreach (var section in this.Sections)
      {
        this.UpdateSection(this, section, inheritedFields);
      }

      // create standard values
      var standardValuesItem = item.Add("__Standard Values", new TemplateID(item.TemplateID));
      if (standardValuesItem == null)
      {
        throw new BuildException(Texts.Text2023, this.TemplateModel.SourceFileName);
      }

      using (new EditContext(item))
      {
        item[FieldIDs.StandardValueHolderId] = standardValuesItem.ID.ToString();
      }
    }

    protected virtual void DeleteFields([NotNull] TemplateSectionBuilder templateSectionBuilder)
    {
      if (templateSectionBuilder.Item == null)
      {
        return;
      }

      foreach (Item child in templateSectionBuilder.Item.Children)
      {
        if (child.TemplateID != TemplateIDs.TemplateField)
        {
          continue;
        }

        var found = false;

        foreach (var field in templateSectionBuilder.Fields)
        {
          if (field.Item == null)
          {
            continue;
          }

          if (field.Item.ID == child.ID)
          {
            found = true;
            break;
          }
        }

        if (!found)
        {
          child.Recycle();
        }
      }
    }

    protected virtual void DeleteSections()
    {
      if (this.Item == null)
      {
        return;
      }

      foreach (Item child in this.Item.Children)
      {
        if (child.TemplateID != TemplateIDs.TemplateSection)
        {
          continue;
        }

        var found = false;

        foreach (var section in this.Sections)
        {
          if (section.Item == null)
          {
            continue;
          }

          if (section.Item.ID == child.ID)
          {
            this.DeleteFields(section);
            found = true;
            break;
          }
        }

        if (!found)
        {
          child.Recycle();
        }
      }
    }

    [NotNull]
    protected IEnumerable<TemplateField> GetInheritedFields([NotNull] TemplateModel templateModel)
    {
      var fields = new List<TemplateField>();

      var database = Factory.GetDatabase(templateModel.DatabaseName);
      var baseTemplates = new List<Item>();

      var templates = templateModel.BaseTemplates.Split(Pipe, StringSplitOptions.RemoveEmptyEntries);
      foreach (var templateId in templates)
      {
        // resolve possible item paths
        var baseTemplateItem = database.GetItem(templateId);
        if (baseTemplateItem == null)
        {
          throw new RetryableBuildException(Texts.Text2037, templateModel.SourceFileName, 0, 0, templateId);
        }

        baseTemplates.Add(baseTemplateItem);

        var template = TemplateManager.GetTemplate(baseTemplateItem.ID, database);
        if (template == null)
        {
          throw new RetryableBuildException(Texts.Text2036, templateModel.SourceFileName, 0, 0, templateId);
        }

        var templateFields = template.GetFields(true);

        foreach (var templateField in templateFields)
        {
          if (fields.All(f => f.Name != templateField.Name))
          {
            fields.Add(templateField);
          }
        }
      }

      templateModel.BaseTemplates = string.Join("|", baseTemplates.Select(t => t.ID.ToString()));

      return fields;
    }

    [CanBeNull]
    protected virtual Item GetParentItem([NotNull] IEmitContext context, [NotNull] Database database)
    {
      Item parentItem = null;

      var parentPath = this.TemplateModel.ItemIdOrPath;
      parentPath = parentPath.Left(parentPath.LastIndexOf('/'));

      if (!string.IsNullOrEmpty(parentPath))
      {
        parentItem = database.GetItem(parentPath);

        if (parentItem == null)
        {
          var innerItem = database.GetItem(TemplateIDs.TemplateFolder);
          if (innerItem != null)
          {
            var templateFolder = new TemplateItem(innerItem);
            parentItem = database.CreateItemPath(parentPath, templateFolder, templateFolder);
          }
        }
      }

      if (parentItem == null)
      {
        parentItem = database.GetItem(ItemIDs.TemplateRoot);
        if (parentItem == null)
        {
          return null;
        }

        var item = parentItem.Children["User Defined"];
        if (item != null)
        {
          parentItem = item;
        }
      }

      return parentItem;
    }

    protected virtual void ResolveItem([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.TemplateModel.DatabaseName);
      if (database == null)
      {
        return;
      }

      if (!string.IsNullOrEmpty(this.TemplateModel.ItemIdOrPath))
      {
        this.Item = database.GetItem(this.TemplateModel.ItemIdOrPath);
      }

      if (this.Item == null)
      {
        return;
      }

      this.TemplateModel.ItemIdOrPath = this.Item.ID.ToString();

      foreach (var section in this.Sections)
      {
        section.ResolveItem(context, this.Item);
      }
    }

    protected virtual void SortFields([NotNull] TemplateSectionBuilder templateSectionBuilder)
    {
      var lastSortorder = 0;

      var fields = templateSectionBuilder.Fields.ToList();
      for (var index = 0; index < fields.Count(); index++)
      {
        var field = fields.ElementAt(index);
        if (field.Item == null)
        {
          continue;
        }

        var sortorder = field.Item.Appearance.Sortorder;

        if (sortorder <= lastSortorder)
        {
          var nextSortorder = lastSortorder + 200;

          if (index < fields.Count() - 1)
          {
            var nextField = fields.ElementAt(index + 1);
            if (nextField.Item != null)
            {
              nextSortorder = nextField.Item.Appearance.Sortorder;
              if (nextSortorder < lastSortorder + 2)
              {
                nextSortorder = lastSortorder + 200;
              }
            }
          }

          sortorder = lastSortorder + ((nextSortorder - lastSortorder) / 2);

          field.Item.Editing.BeginEdit();
          field.Item.Appearance.Sortorder = sortorder;
          field.Item.Editing.EndEdit();
        }

        lastSortorder = sortorder;
      }
    }

    protected virtual void SortSections([NotNull] TemplateBuilder templateBuilder)
    {
      var lastSortorder = 0;

      var sections = templateBuilder.Sections.ToList();
      for (var index = 0; index < sections.Count(); index++)
      {
        var section = sections.ElementAt(index);
        if (section.Item == null)
        {
          continue;
        }

        var sortorder = section.Item.Appearance.Sortorder;

        if (sortorder <= lastSortorder)
        {
          var nextSortorder = lastSortorder + 200;

          if (index < sections.Count() - 1)
          {
            var nextSection = sections.ElementAt(index + 1);
            if (nextSection.Item != null)
            {
              nextSortorder = nextSection.Item.Appearance.Sortorder;
              if (nextSortorder < lastSortorder + 2)
              {
                nextSortorder = lastSortorder + 200;
              }
            }
          }

          sortorder = lastSortorder + ((nextSortorder - lastSortorder) / 2);

          using (new EditContext(section.Item))
          {
            section.Item.Appearance.Sortorder = sortorder;
          }
        }

        this.SortFields(section);

        lastSortorder = sortorder;
      }
    }

    protected virtual void UpdateField([NotNull] TemplateSectionBuilder templateSectionBuilder, [NotNull] TemplateFieldBuilder templateFieldBuilder, [NotNull] IEnumerable<TemplateField> inheritedFields)
    {
      if (inheritedFields.Any(f => string.Compare(f.Name, templateFieldBuilder.TemplaterFieldModel.Name, StringComparison.OrdinalIgnoreCase) == 0))
      {
        return;
      }

      var item = templateFieldBuilder.Item;
      if (item == null)
      {
        item = ItemManager.AddFromTemplate(templateFieldBuilder.TemplaterFieldModel.Name, new TemplateID(TemplateIDs.TemplateField), templateSectionBuilder.Item);
        if (item == null)
        {
          // todo: report error
          return;
        }

        templateFieldBuilder.Item = item;
      }
      else if (templateSectionBuilder.Item != null && item.ParentID != templateSectionBuilder.Item.ID)
      {
        item.MoveTo(templateSectionBuilder.Item);
      }

      using (new EditContext(item))
      {
        item.Name = templateFieldBuilder.TemplaterFieldModel.Name;
        item["Type"] = templateFieldBuilder.TemplaterFieldModel.Type;
        item["Shared"] = templateFieldBuilder.TemplaterFieldModel.Shared ? "1" : string.Empty;
        item["Unversioned"] = templateFieldBuilder.TemplaterFieldModel.Unversioned ? "1" : string.Empty;
        item["Source"] = templateFieldBuilder.TemplaterFieldModel.Source;
      }
    }

    protected virtual void UpdateSection([NotNull] TemplateBuilder templateBuilder, [NotNull] TemplateSectionBuilder templateSectionBuilder, [NotNull] IEnumerable<TemplateField> inheritedFields)
    {
      if (templateSectionBuilder.Item == null)
      {
        templateSectionBuilder.Item = ItemManager.AddFromTemplate(templateSectionBuilder.TemplateSectionModel.Name, new TemplateID(TemplateIDs.TemplateSection), templateBuilder.Item);
        if (templateSectionBuilder.Item == null)
        {
          // todo: report error 
          return;
        }
      }
      else
      {
        if (templateBuilder.Item != null && templateSectionBuilder.Item.ParentID != templateBuilder.Item.ID)
        {
          templateSectionBuilder.Item.MoveTo(templateBuilder.Item);
        }

        if (templateSectionBuilder.Item.Name != templateSectionBuilder.TemplateSectionModel.Name)
        {
          using (new EditContext(templateSectionBuilder.Item))
          {
            templateSectionBuilder.Item.Name = templateSectionBuilder.TemplateSectionModel.Name;
          }
        }
      }

      foreach (var field in templateSectionBuilder.Fields)
      {
        this.UpdateField(templateSectionBuilder, field, inheritedFields);
      }
    }

    protected virtual void UpdateTemplate([NotNull] IEnumerable<TemplateField> inheritedFields)
    {
      var item = this.Item;
      if (item == null)
      {
        return;
      }

      using (new EditContext(item))
      {
        item[FieldIDs.BaseTemplate] = this.TemplateModel.BaseTemplates;
        item.Name = this.TemplateModel.Name;

        if (!string.IsNullOrEmpty(this.TemplateModel.Icon))
        {
          item.Appearance.Icon = this.TemplateModel.Icon;
        }
      }

      foreach (var section in this.Sections)
      {
        this.UpdateSection(this, section, inheritedFields);
      }
    }
  }
}
