namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders.ElementParsers
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.Linq;
  using System.Xml.Linq;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Extensions.StringExtensions;
  using Sitecore.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Models.Items;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IElementParser))]
  public class ItemParser : ElementParserBase
  {
    public override bool CanParse(XmlItemParserContext context, XElement element)
    {
      return element.Name.LocalName == "Item";
    }

    public override void Parse(XmlItemParserContext context, XElement element)
    {
      var itemModel = new ItemModel();
      context.BuildContext.BuildContext.Objects.Add(itemModel);

      itemModel.Name = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(itemModel.Name))
      {
        itemModel.Name = context.ItemName;
      }

      itemModel.DatabaseName = context.DatabaseName;
      itemModel.ItemIdOrPath = context.ParentItemPath + "/" + itemModel.Name;
      itemModel.TemplateIdOrPath = this.GetTemplateIdOrPath(context, element);
      itemModel.SourceElement = element;

      if (!string.IsNullOrEmpty(element.GetAttributeValue("Template.Create")))
      {
        var templateBuilder = this.ParseTemplate(context, element);
        itemModel.TemplateIdOrPath = templateBuilder.ItemIdOrPath;
      }

      this.ParseChildElements(context, itemModel, element);
    }

    [NotNull]
    protected IEnumerable<TemplateField> GetInheritedFields([NotNull] XmlItemParserContext context, [NotNull] string baseTemplates)
    {
      var fields = new List<TemplateField>();

      var database = Factory.GetDatabase(context.DatabaseName);

      var templates = baseTemplates.Split('|');
      foreach (var templateId in templates)
      {
        var template = TemplateManager.GetTemplate(new ID(templateId), database);
        var templateFields = template.GetFields(true);

        foreach (var templateField in templateFields)
        {
          if (fields.All(f => f.Name != templateField.Name))
          {
            fields.Add(templateField);
          }
        }
      }

      return fields;
    }

    [NotNull]
    protected virtual string GetTemplateIdOrPath([NotNull] XmlItemParserContext context, [NotNull] XElement element)
    {
      var templateIdOrPath = element.GetAttributeValue("Template") ?? string.Empty;
      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        templateIdOrPath = element.GetAttributeValue("Template.Create") ?? string.Empty;
      }

      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        return string.Empty;
      }

      // return if absolute path or guid
      templateIdOrPath = templateIdOrPath.Trim();
      if (templateIdOrPath.StartsWith("/") || templateIdOrPath.StartsWith("{"))
      {
        return templateIdOrPath;
      }

      // resolve relative paths
      templateIdOrPath = PathHelper.NormalizeWebPath(PathHelper.Combine(context.ItemPath, templateIdOrPath));

      return templateIdOrPath;
    }

    protected virtual void ParseChildElements([NotNull] XmlItemParserContext context, [NotNull] ItemModel itemModel, [NotNull] XElement element)
    {
      var itemPath = context.ItemPath;

      foreach (var child in element.Elements())
      {
        context.ItemPath = itemModel.ItemIdOrPath + "/" + child.Name.LocalName;

        if (child.Name.LocalName == "Field")
        {
          this.ParseFieldElement(context, itemModel, child);
        }
        else
        {
          context.ElementParser.ParseElement(context, child);
        }
      }

      context.ItemPath = itemPath;
    }

    protected virtual void ParseFieldElement([NotNull] XmlItemParserContext context, [NotNull] ItemModel itemModel, [NotNull] XElement fieldElement)
    {
      var fieldName = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        throw new BuildException(Texts.Text2011, context.BuildContext.FileName, fieldElement);
      }

      var field = itemModel.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        throw new BuildException(Texts.Text2012, context.BuildContext.FileName, fieldElement, fieldName);
      }

      var value = fieldElement.GetAttributeValue("Value");
      if (string.IsNullOrEmpty(value))
      {
        value = fieldElement.Value;
      }

      field = new FieldModel();
      itemModel.Fields.Add(field);

      field.Name = fieldName;
      field.Value = value;
      field.SourceElement = fieldElement;
    }

    [NotNull]
    protected virtual TemplateModel ParseTemplate([NotNull] XmlItemParserContext context, [NotNull] XElement element)
    {
      var templateBuilder = new TemplateModel();
      context.Templates.Add(templateBuilder);

      templateBuilder.ItemIdOrPath = this.GetTemplateIdOrPath(context, element);
      if (string.IsNullOrEmpty(templateBuilder.ItemIdOrPath))
      {
        throw new BuildException(Texts.Text2010, context.BuildContext.FileName, element);
      }

      templateBuilder.DatabaseName = context.DatabaseName;
      templateBuilder.Icon = element.GetAttributeValue("Template.Icon");
      templateBuilder.BaseTemplates = element.GetAttributeValue("Template.BaseTemplates");
      if (string.IsNullOrEmpty(templateBuilder.BaseTemplates))
      {
        templateBuilder.BaseTemplates = TemplateIDs.StandardTemplate.ToString();
      }

      // get template name
      var n = templateBuilder.ItemIdOrPath.LastIndexOf('/');
      templateBuilder.Name = templateBuilder.ItemIdOrPath.Mid(n + 1);

      var sectionBuilder = new TemplateSectionModel();
      templateBuilder.Sections.Add(sectionBuilder);
      sectionBuilder.Name = "Fields";

      var inheritedFields = this.GetInheritedFields(context, templateBuilder.BaseTemplates).ToList();

      foreach (var child in element.Elements())
      {
        if (child.Name.LocalName != "Field")
        {
          throw new BuildException(Texts.Text2015, context.BuildContext.FileName, child);
        }

        var name = child.GetAttributeValue("Name");
        if (inheritedFields.Any(f => string.Compare(f.Name, name, StringComparison.OrdinalIgnoreCase) == 0))
        {
          continue;
        }

        var fieldModel = new TemplateFieldModel();
        sectionBuilder.Fields.Add(fieldModel);
        fieldModel.Name = name;
        fieldModel.Type = child.GetAttributeValue("Field.Type");
        if (string.IsNullOrEmpty(fieldModel.Type))
        {
          fieldModel.Type = "Single-Line Text";
        }

        fieldModel.Shared = child.GetAttributeValue("Field.Sharing") == "Shared";
        fieldModel.Unversioned = child.GetAttributeValue("Field.Sharing") == "Unversioned";
        fieldModel.Source = child.GetAttributeValue("Field.Source") ?? string.Empty;
      }

      return templateBuilder;
    }
  }
}
