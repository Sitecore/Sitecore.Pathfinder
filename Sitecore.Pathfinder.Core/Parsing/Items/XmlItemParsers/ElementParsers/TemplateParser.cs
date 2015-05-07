namespace Sitecore.Pathfinder.Parsing.Items.XmlItemParsers.ElementParsers
{
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IElementParser))]
  public class TemplateParser : ElementParserBase
  {
    public override bool CanParse(IItemParseContext context, XmlItemParser parser, XElement element)
    {
      return element.Name.LocalName == "Template";
    }

    public override void Parse(IItemParseContext context, XmlItemParser parser, XElement element)
    {
      var templateModel = new TemplateModel(context.FileName);
      context.ParseContext.Project.Models.Add(templateModel);

      templateModel.Name = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        templateModel.Name = context.ItemName;
      }

      templateModel.DatabaseName = context.DatabaseName;
      templateModel.ItemIdOrPath = context.ParentItemPath + "/" + templateModel.Name;
      templateModel.BaseTemplates = element.GetAttributeValue("BaseTemplates") ?? string.Empty;
      templateModel.Icon = element.GetAttributeValue("Icon") ?? string.Empty;

      foreach (var sectionElement in element.Elements())
      {
        this.ParseSection(context, templateModel, sectionElement);
      }
    }

    protected virtual void ParseField([NotNull] IItemParseContext context, [NotNull] TemplateSectionModel templateSectionModel, [NotNull] XElement fieldElement)
    {
      var templateFieldModel = new TemplateFieldModel();
      templateSectionModel.Fields.Add(templateFieldModel);

      templateFieldModel.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateFieldModel.Name))
      {
        throw new BuildException(Texts.Text2008, context.FileName, fieldElement);
      }

      templateFieldModel.Shared = fieldElement.GetAttributeValue("Sharing") == "Shared";
      templateFieldModel.Unversioned = fieldElement.GetAttributeValue("Sharing") == "Unversioned";
      templateFieldModel.Source = fieldElement.GetAttributeValue("Source") ?? string.Empty;

      templateFieldModel.Type = fieldElement.GetAttributeValue("Type");
      if (string.IsNullOrEmpty(templateFieldModel.Type))
      {
        templateFieldModel.Type = "Single-Line Text";
      }
    }

    protected virtual void ParseSection([NotNull] IItemParseContext context, [NotNull] TemplateModel templateModel, [NotNull] XElement sectionElement)
    {
      var templateSectionModel = new TemplateSectionModel();
      templateModel.Sections.Add(templateSectionModel);

      templateSectionModel.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        throw new BuildException(Texts.Text2007, context.FileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, templateSectionModel, fieldElement);
      }
    }
  }
}
