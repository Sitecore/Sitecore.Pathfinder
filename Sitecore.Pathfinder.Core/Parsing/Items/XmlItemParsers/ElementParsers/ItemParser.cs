namespace Sitecore.Pathfinder.Parsing.Items.XmlItemParsers.ElementParsers
{
  using System;
  using System.ComponentModel.Composition;
  using System.Linq;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IElementParser))]
  public class ItemParser : ElementParserBase
  {
    public override bool CanParse(IItemParseContext context, XmlItemParser parser, XElement element)
    {
      return element.Name.LocalName == "Item";
    }

    public override void Parse(IItemParseContext context, XmlItemParser parser, XElement element)
    {
      var itemModel = new ItemModel(context.FileName);
      context.ParseContext.Project.Elements.Add(itemModel);

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
        var templateModel = this.ParseTemplate(context, element);
        itemModel.TemplateIdOrPath = templateModel.ItemIdOrPath;
      }

      this.ParseChildElements(context, parser, itemModel, element);
    }

    [NotNull]
    protected virtual string GetTemplateIdOrPath([NotNull] IItemParseContext context, [NotNull] XElement element)
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

    protected virtual void ParseChildElements([NotNull] IItemParseContext context, XmlItemParser parser, [NotNull] ItemModel itemModel, [NotNull] XElement element)
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
          parser.ParseElement(context, child);
        }
      }

      context.ItemPath = itemPath;
    }

    protected virtual void ParseFieldElement([NotNull] IItemParseContext context, [NotNull] ItemModel itemModel, [NotNull] XElement fieldElement)
    {
      var fieldName = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        throw new BuildException(Texts.Text2011, context.FileName, fieldElement);
      }

      var field = itemModel.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        throw new BuildException(Texts.Text2012, context.FileName, fieldElement, fieldName);
      }

      var value = fieldElement.GetAttributeValue("Value");
      if (string.IsNullOrEmpty(value))
      {
        value = fieldElement.Value;
      }

      field = new FieldModel(itemModel.SourceFileName);
      itemModel.Fields.Add(field);

      field.Name = fieldName;
      field.Value = value;
      field.SourceElement = fieldElement;
    }

    [NotNull]
    protected virtual TemplateModel ParseTemplate([NotNull] IItemParseContext context, [NotNull] XElement element)
    {
      var templateBuilder = new TemplateModel(context.FileName);
      context.ParseContext.Project.Elements.Add(templateBuilder);

      templateBuilder.ItemIdOrPath = this.GetTemplateIdOrPath(context, element);
      if (string.IsNullOrEmpty(templateBuilder.ItemIdOrPath))
      {
        throw new BuildException(Texts.Text2010, context.FileName, element);
      }

      templateBuilder.DatabaseName = context.DatabaseName;
      templateBuilder.Icon = element.GetAttributeValue("Template.Icon");
      templateBuilder.BaseTemplates = element.GetAttributeValue("Template.BaseTemplates");
      if (string.IsNullOrEmpty(templateBuilder.BaseTemplates))
      {
        templateBuilder.BaseTemplates = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";
      }

      // get template name
      var n = templateBuilder.ItemIdOrPath.LastIndexOf('/');
      templateBuilder.Name = templateBuilder.ItemIdOrPath.Mid(n + 1);

      var sectionBuilder = new TemplateSectionModel();
      templateBuilder.Sections.Add(sectionBuilder);
      sectionBuilder.Name = "Fields";

      foreach (var child in element.Elements())
      {
        if (child.Name.LocalName != "Field")
        {
          throw new BuildException(Texts.Text2015, context.FileName, child);
        }

        var name = child.GetAttributeValue("Name");

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
