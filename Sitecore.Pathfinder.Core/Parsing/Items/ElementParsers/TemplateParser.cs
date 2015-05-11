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
      var template = new Template(context.ParseContext.Project, context.ParseContext.SourceFile);
      context.ParseContext.Project.Items.Add(template);

      template.ItemName = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        template.ItemName = context.ParseContext.ItemName;
      }

      template.DatabaseName = context.ParseContext.DatabaseName;
      template.ItemIdOrPath = context.ParentItemPath + "/" + template.ItemName;
      template.BaseTemplates = element.GetAttributeValue("BaseTemplates");
      template.Icon = element.GetAttributeValue("Icon");

      foreach (var sectionElement in element.Elements())
      {
        this.ParseSection(context, template, sectionElement);
      }
    }

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] TemplateSection templateSection, [NotNull] XElement fieldElement)
    {
      var templateField = new TemplateField();
      templateSection.Fields.Add(templateField);

      templateField.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateField.Name))
      {
        throw new BuildException(Texts.Text2008, context.ParseContext.SourceFile.SourceFileName, fieldElement);
      }

      templateField.Shared = fieldElement.GetAttributeValue("Sharing") == "Shared";
      templateField.Unversioned = fieldElement.GetAttributeValue("Sharing") == "Unversioned";
      templateField.Source = fieldElement.GetAttributeValue("Source");

      templateField.Type = fieldElement.GetAttributeValue("Type");
      if (string.IsNullOrEmpty(templateField.Type))
      {
        templateField.Type = "Single-Line Text";
      }
    }

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] XElement sectionElement)
    {
      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);

      templateSection.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, context.ParseContext.SourceFile.SourceFileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, templateSection, fieldElement);
      }
    }
  }
}
