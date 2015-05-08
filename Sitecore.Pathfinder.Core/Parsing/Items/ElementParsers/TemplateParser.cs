namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System.ComponentModel.Composition;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IElementParser))]
  public class TemplateParser : ElementParserBase
  {
    public override bool CanParse(ItemParseContext context, XElement element)
    {
      return element.Name.LocalName == "Template";
    }

    public override void Parse(ItemParseContext context, XElement element)
    {
      var templateModel = new Template(context.ParseContext.SourceFile);
      context.ParseContext.Project.Items.Add(templateModel);

      templateModel.ItemName = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.ItemName))
      {
        templateModel.ItemName = context.ParseContext.ItemName;
      }

      templateModel.DatabaseName = context.ParseContext.DatabaseName;
      templateModel.ItemIdOrPath = context.ParentItemPath + "/" + templateModel.ItemName;
      templateModel.BaseTemplates = element.GetAttributeValue("BaseTemplates") ?? string.Empty;
      templateModel.Icon = element.GetAttributeValue("Icon") ?? string.Empty;

      foreach (var sectionElement in element.Elements())
      {
        this.ParseSection(context, templateModel, sectionElement);
      }
    }

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] TemplateSection templateSection, [NotNull] XElement fieldElement)
    {
      var templateFieldModel = new TemplateField();
      templateSection.Fields.Add(templateFieldModel);

      templateFieldModel.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateFieldModel.Name))
      {
        throw new BuildException(Texts.Text2008, context.ParseContext.SourceFile.SourceFileName, fieldElement);
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

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] XElement sectionElement)
    {
      var templateSectionModel = new TemplateSection();
      template.Sections.Add(templateSectionModel);

      templateSectionModel.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, context.ParseContext.SourceFile.SourceFileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, templateSectionModel, fieldElement);
      }
    }
  }
}
