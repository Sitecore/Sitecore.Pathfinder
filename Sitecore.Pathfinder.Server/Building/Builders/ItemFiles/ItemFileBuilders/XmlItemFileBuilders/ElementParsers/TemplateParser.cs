namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.XmlItemFileBuilders.ElementParsers
{
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Data;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IElementParser))]
  public class TemplateParser : ElementParserBase
  {
    public override bool CanParse(XmlItemParserContext context, XElement element)
    {
      return element.Name.LocalName == "Template";
    }

    public override void Parse(XmlItemParserContext context, XElement element)
    {
      var templateModel = new TemplateModel();
      context.Templates.Add(templateModel);

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

    protected virtual void ParseField([NotNull] XmlItemParserContext context, [NotNull] TemplateSectionModel sectionModel, [NotNull] XElement fieldElement)
    {
      var fieldModel = new TemplateFieldModel();
      sectionModel.Fields.Add(fieldModel);

      fieldModel.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldModel.Name))
      {
        throw new BuildException(Texts.Text2008, context.BuildContext.FileName, fieldElement);
      }

      fieldModel.Shared = fieldElement.GetAttributeValue("Sharing") == "Shared";
      fieldModel.Unversioned = fieldElement.GetAttributeValue("Sharing") == "Unversioned";
      fieldModel.Source = fieldElement.GetAttributeValue("Source") ?? string.Empty;

      fieldModel.Type = fieldElement.GetAttributeValue("Type");
      if (string.IsNullOrEmpty(fieldModel.Type))
      {
        fieldModel.Type = "Single-Line Text";
      }
    }

    protected virtual void ParseSection([NotNull] XmlItemParserContext context, [NotNull] TemplateModel templateModel, [NotNull] XElement sectionElement)
    {
      var sectionModel = new TemplateSectionModel();
      templateModel.Sections.Add(sectionModel);

      sectionModel.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        throw new BuildException(Texts.Text2007, context.BuildContext.FileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, sectionModel, fieldElement);
      }
    }
  }
}
