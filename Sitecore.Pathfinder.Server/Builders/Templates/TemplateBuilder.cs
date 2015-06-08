namespace Sitecore.Pathfinder.Builders.Templates
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Templates;

  public class TemplateBuilder
  {
    [CanBeNull]
    private IEnumerable<TemplateSectionBuilder> sectionBuilders;

    public TemplateBuilder([NotNull] Template template)
    {
      this.Template = template;
    }

    [CanBeNull]
    public Item Item { get; set; }

    [NotNull]
    public IEnumerable<TemplateSectionBuilder> Sections
    {
      get
      {
        return this.sectionBuilders ?? (this.sectionBuilders = this.Template.Sections.Select(s => new TemplateSectionBuilder(s)).ToList());
      }
    }

    [NotNull]
    public Template Template { get; }

    [CanBeNull]
    public Item Build([NotNull] IEmitContext context)
    {
      var inheritedFields = this.GetInheritedFields(this.Template);

      if (this.Item == null)
      {
        this.ResolveItem(context);
      }

      if (this.Item == null)
      {
        this.CreateNewTemplate(context, inheritedFields);
        if (this.Item == null)
        {
          return null;
        }

        context.RegisterAddedItem(this.Item);
      }
      else
      {
        this.UpdateTemplate(context, inheritedFields);
        this.DeleteSections(context);
      }

      this.SortSections(context, this);

      return this.Item;
    }

    public void CreateStandardValuesItem([NotNull] IEmitContext context)
    {
      var item = this.Item;
      if (item == null)
      {
        throw new EmitException(Texts.Template_missing);
      }

      var standardValuesItem = item.Add("__Standard Values", new TemplateID(item.TemplateID));
      if (standardValuesItem == null)
      {
        throw new EmitException(Texts.Failed_to_add_new_template, this.Template.Snapshot);
      }

      // update standard value link
      using (new EditContext(item))
      {
        item[FieldIDs.StandardValueHolderId] = standardValuesItem.ID.ToString();
      }

      // update field values
      using (new EditContext(standardValuesItem))
      {
        foreach (var section in this.Sections)
        {
          foreach (var field in section.Fields)
          {
            if (!string.IsNullOrEmpty(field.TemplaterField.StandardValue))
            {
              standardValuesItem[field.TemplaterField.Name] = field.TemplaterField.StandardValue;
            }
          }
        }
      }
    }

    protected virtual void CreateNewTemplate([NotNull] IEmitContext context, [NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
    {
      var database = context.DataService.GetDatabase(this.Template.DatabaseName);
      if (database == null)
      {
        return;
      }

      var parentItem = this.GetParentItem(context, database);
      if (parentItem == null)
      {
        throw new RetryableEmitException(Texts.Failed_to_create_template, this.Template.Snapshot);
      }

      var item = ItemManager.AddFromTemplate(this.Template.ItemName.Value, new TemplateID(TemplateIDs.Template), parentItem, new ID(this.Template.Guid));
      if (item == null)
      {
        throw new EmitException(Texts.Failed_to_add_new_template, this.Template.Snapshot);
      }

      this.Item = item;
      using (new EditContext(item))
      {
        if (!string.IsNullOrEmpty(this.Template.BaseTemplates))
        {
          item[FieldIDs.BaseTemplate] = this.Template.BaseTemplates;
        }

        if (!string.IsNullOrEmpty(this.Template.Icon))
        {
          item.Appearance.Icon = this.Template.Icon;
        }

        if (!string.IsNullOrEmpty(this.Template.ShortHelp))
        {
          item.Help.ToolTip = this.Template.ShortHelp;
        }

        if (!string.IsNullOrEmpty(this.Template.LongHelp))
        {
          item.Help.Text = this.Template.LongHelp;
        }
      }

      foreach (var section in this.Sections)
      {
        this.UpdateSection(context, section, inheritedFields);
      }

      // create standard values
      var standardValuesItem = item.Add("__Standard Values", new TemplateID(item.TemplateID));
      if (standardValuesItem == null)
      {
        throw new EmitException(Texts.Failed_to_add_new_template, this.Template.Snapshot);
      }

      using (new EditContext(item))
      {
        item[FieldIDs.StandardValueHolderId] = standardValuesItem.ID.ToString();
      }

      context.RegisterAddedItem(standardValuesItem);
    }

    protected virtual void DeleteFields([NotNull] IEmitContext context, [NotNull] TemplateSectionBuilder templateSectionBuilder)
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
          context.RegisterDeletedItem(child);
          child.Recycle();
        }
      }
    }

    protected virtual void DeleteSections([NotNull] IEmitContext context)
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
            this.DeleteFields(context, section);
            found = true;
            break;
          }
        }

        if (!found)
        {
          context.RegisterDeletedItem(child);
          child.Recycle();
        }
      }
    }

    [NotNull]
    protected IEnumerable<Sitecore.Data.Templates.TemplateField> GetInheritedFields([NotNull] Template template)
    {
      var fields = new List<Sitecore.Data.Templates.TemplateField>();

      var database = Factory.GetDatabase(template.DatabaseName);
      var baseTemplates = new List<Item>();

      var templates = template.BaseTemplates.Split(Constants.Pipe, StringSplitOptions.RemoveEmptyEntries);
      foreach (var templateId in templates)
      {
        // resolve possible item paths
        var baseTemplateItem = database.GetItem(templateId);
        if (baseTemplateItem == null)
        {
          throw new RetryableEmitException(Texts.Base_Template_missing, template.Snapshot, templateId);
        }

        baseTemplates.Add(baseTemplateItem);

        var t = TemplateManager.GetTemplate(baseTemplateItem.ID, database);
        if (t == null)
        {
          throw new RetryableEmitException(Texts.Template_missing, template.Snapshot, templateId);
        }

        var templateFields = t.GetFields(true);

        foreach (var templateField in templateFields)
        {
          if (fields.All(f => f.Name != templateField.Name))
          {
            fields.Add(templateField);
          }
        }
      }

      template.BaseTemplates = string.Join("|", baseTemplates.Select(t => t.ID.ToString()));

      return fields;
    }

    [CanBeNull]
    protected virtual Item GetParentItem([NotNull] IEmitContext context, [NotNull] Database database)
    {
      var parentPath = PathHelper.GetItemParentPath(this.Template.ItemIdOrPath);
      if (string.IsNullOrEmpty(parentPath))
      {
        return null;
      }

      var parentItem = database.GetItem(parentPath);
      if (parentItem == null)
      {
        var innerItem = database.GetItem(TemplateIDs.TemplateFolder);
        if (innerItem != null)
        {
          var templateFolder = new TemplateItem(innerItem);
          parentItem = database.CreateItemPath(parentPath, templateFolder, templateFolder);
        }
      }

      return parentItem;
    }

    protected virtual void ResolveItem([NotNull] IEmitContext context)
    {
      var database = context.DataService.GetDatabase(this.Template.DatabaseName);
      if (database == null)
      {
        return;
      }

      this.Item = database.GetItem(new ID(this.Template.Guid));
      if (this.Item == null)
      {
        return;
      }

      foreach (var section in this.Sections)
      {
        section.ResolveItem(context, this.Item);
      }
    }

    protected virtual void SortFields([NotNull] IEmitContext context, [NotNull] TemplateSectionBuilder templateSectionBuilder)
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

          context.RegisterUpdatedItem(field.Item);

          using (new EditContext(field.Item))
          {
          field.Item.Editing.EndEdit();
          }
        }

        lastSortorder = sortorder;
      }
    }

    protected virtual void SortSections([NotNull] IEmitContext context, [NotNull] TemplateBuilder templateBuilder)
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

          context.RegisterUpdatedItem(section.Item);

          using (new EditContext(section.Item))
          {
            section.Item.Appearance.Sortorder = sortorder;
          }
        }

        this.SortFields(context, section);

        lastSortorder = sortorder;
      }
    }

    protected virtual void UpdateField([NotNull] IEmitContext context, [NotNull] TemplateSectionBuilder templateSectionBuilder, [NotNull] TemplateFieldBuilder templateFieldBuilder, [NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
    {
      if (inheritedFields.Any(f => string.Compare(f.Name, templateFieldBuilder.TemplaterField.Name, StringComparison.OrdinalIgnoreCase) == 0))
      {
        return;
      }

      var item = templateFieldBuilder.Item;

      var isNew = item == null;
      if (isNew)
      {
        item = ItemManager.AddFromTemplate(templateFieldBuilder.TemplaterField.Name, new TemplateID(TemplateIDs.TemplateField), templateSectionBuilder.Item);
        if (item == null)
        {
          throw new EmitException(Texts.Could_not_create_template_field, this.Template.ItemName.Source ?? TextNode.Empty, templateFieldBuilder.TemplaterField.Name);
        }

        templateFieldBuilder.Item = item;
      }
      else
      {
        context.RegisterUpdatedItem(item);
      }

      if (templateSectionBuilder.Item != null && item.ParentID != templateSectionBuilder.Item.ID)
      {
        item.MoveTo(templateSectionBuilder.Item);
      }

      using (new EditContext(item))
      {
        if (!string.IsNullOrEmpty(templateFieldBuilder.TemplaterField.Name))
        {
          item.Name = templateFieldBuilder.TemplaterField.Name;
        }

        if (!string.IsNullOrEmpty(templateFieldBuilder.TemplaterField.Type))
        {
          item["Type"] = templateFieldBuilder.TemplaterField.Type;
        }

        item["Shared"] = templateFieldBuilder.TemplaterField.Shared ? "1" : string.Empty;
        item["Unversioned"] = templateFieldBuilder.TemplaterField.Unversioned ? "1" : string.Empty;

        if (!string.IsNullOrEmpty(templateFieldBuilder.TemplaterField.Source))
        {
          item["Source"] = templateFieldBuilder.TemplaterField.Source;
        }

        if (!string.IsNullOrEmpty(templateFieldBuilder.TemplaterField.ShortHelp))
        {
          item.Help.ToolTip = templateFieldBuilder.TemplaterField.ShortHelp;
        }

        if (!string.IsNullOrEmpty(templateFieldBuilder.TemplaterField.LongHelp))
        {
          item.Help.Text = templateFieldBuilder.TemplaterField.LongHelp;
        }

        item.Appearance.Sortorder = templateFieldBuilder.TemplaterField.SortOrder;
      }

      if (isNew)
      {
        context.RegisterAddedItem(item);
      }
    }

    protected virtual void UpdateSection([NotNull] IEmitContext context, [NotNull] TemplateSectionBuilder templateSectionBuilder, [NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
    {
      if (this.Item == null)
      {
        return;
      }

      var isNew = templateSectionBuilder.Item == null;
      if (isNew)
      {
        templateSectionBuilder.Item = ItemManager.AddFromTemplate(templateSectionBuilder.TemplateSection.Name, new TemplateID(TemplateIDs.TemplateSection), this.Item);
        if (templateSectionBuilder.Item == null)
        {
          throw new EmitException(Texts.Could_not_create_section_item, this.Template.ItemName.Source ?? TextNode.Empty);
        }
      }
      else
      {
        context.RegisterUpdatedItem(templateSectionBuilder.Item);
      }

      if (this.Item != null && templateSectionBuilder.Item.ParentID != this.Item.ID)
      {
        templateSectionBuilder.Item.MoveTo(this.Item);
      }

      using (new EditContext(templateSectionBuilder.Item))
      {
        if (templateSectionBuilder.Item.Name != templateSectionBuilder.TemplateSection.Name)
        {
          templateSectionBuilder.Item.Name = templateSectionBuilder.TemplateSection.Name;
        }

        if (!string.IsNullOrEmpty(templateSectionBuilder.TemplateSection.Icon))
        {
          templateSectionBuilder.Item.Appearance.Icon = templateSectionBuilder.TemplateSection.Icon;
        }
      }

      foreach (var field in templateSectionBuilder.Fields)
      {
        this.UpdateField(context, templateSectionBuilder, field, inheritedFields);
      }

      if (isNew)
      {
        context.RegisterAddedItem(templateSectionBuilder.Item);
      }
    }

    protected virtual void UpdateTemplate([NotNull] IEmitContext context, [NotNull] IEnumerable<Sitecore.Data.Templates.TemplateField> inheritedFields)
    {
      var item = this.Item;
      if (item == null)
      {
        return;
      }

      context.RegisterUpdatedItem(item);

      // move
      if (string.Compare(item.Paths.Path, this.Template.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(item.ID.ToString(), this.Template.ItemIdOrPath, StringComparison.OrdinalIgnoreCase) != 0)
      {
        var parentItemPath = PathHelper.GetItemParentPath(this.Template.ItemIdOrPath);

        var parentItem = item.Database.GetItem(parentItemPath);
        if (parentItem == null)
        {
          parentItem = item.Database.CreateItemPath(parentItemPath);
          if (parentItem == null)
          {
            throw new RetryableEmitException(Texts.Could_not_create_item, this.Template.Snapshot, parentItemPath);
          }
        }

        item.MoveTo(parentItem);
      }

      // rename and update fields
      using (new EditContext(item))
      {
        if (item.Name != this.Template.ItemName.Value)
        {
          item.Name = this.Template.ItemName.Value;
        }

        if (!string.IsNullOrEmpty(this.Template.BaseTemplates))
        {
          item[FieldIDs.BaseTemplate] = this.Template.BaseTemplates;
        }

        if (!string.IsNullOrEmpty(this.Template.Icon))
        {
          item.Appearance.Icon = this.Template.Icon;
        }

        if (!string.IsNullOrEmpty(this.Template.ShortHelp))
        {
          item.Help.ToolTip = this.Template.ShortHelp;
        }

        if (!string.IsNullOrEmpty(this.Template.LongHelp))
        {
          item.Help.Text = this.Template.LongHelp;
        }
      }

      foreach (var templateSectionBuilder in this.Sections)
      {
        this.UpdateSection(context, templateSectionBuilder, inheritedFields);
      }
    }
  }
}
