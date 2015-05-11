namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Other;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(IParser))]
  public class ComponentParser : ParserBase
  {
    private const string FileExtension = ".component.xml";

    public ComponentParser() : base(Items)
    {
    }

    public override bool CanParse(IParseContext context)
    {
      return context.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var root = context.SourceFile.ReadAsXml(context);

      var privateTemplate = this.CreatePrivateTemplate(context, context.SourceFile, root);
      if (privateTemplate == null)
      {
        throw new BuildException(Texts.Text2031);
      }

      var publicTemplate = this.CreatePublicTemplate(context, context.SourceFile, privateTemplate);

      var component = new Component(context.Project, context.SourceFile, privateTemplate, publicTemplate);
      context.Project.Items.Add(component);
    }

    [NotNull]
    protected Template CreatePrivateTemplate([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile, [NotNull] XElement root)
    {
      var privateTemplate = new Template(context.Project, sourceFile);
      context.Project.Items.Add(privateTemplate);

      // todo: remove duplicated code from TemplateParser
      this.Parse(context, sourceFile, privateTemplate, root);

      privateTemplate.ItemName = "__" + privateTemplate.ItemName;
      var n = privateTemplate.ItemIdOrPath.LastIndexOf('/');
      privateTemplate.ItemIdOrPath = privateTemplate.ItemIdOrPath.Left(n + 1) + privateTemplate.ItemName;

      return privateTemplate;
    }

    [NotNull]
    protected Template CreatePublicTemplate([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile, [NotNull] Template privateTemplate)
    {
      var publicTemplate = new Template(context.Project, sourceFile);
      context.Project.Items.Add(publicTemplate);

      publicTemplate.ItemName = privateTemplate.ItemName.Mid(2);
      publicTemplate.DatabaseName = privateTemplate.DatabaseName;
      publicTemplate.ItemIdOrPath = PathHelper.NormalizeItemPath(Path.GetDirectoryName(privateTemplate.ItemIdOrPath)) + "/" + publicTemplate.ItemName;
      publicTemplate.BaseTemplates = privateTemplate.ItemIdOrPath;

      return publicTemplate;
    }

    protected void Parse([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile, [NotNull] Template template, [NotNull] XElement element)
    {
      template.ItemName = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        template.ItemName = context.ItemName;
      }

      template.DatabaseName = context.DatabaseName;
      template.ItemIdOrPath = context.ItemPath;
      template.BaseTemplates = element.GetAttributeValue("BaseTemplates") ?? string.Empty;
      template.Icon = element.GetAttributeValue("Icon") ?? string.Empty;

      foreach (var sectionElement in element.Elements())
      {
        this.ParseSection(context, sourceFile, template, sectionElement);
      }
    }

    protected void ParseField([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile, [NotNull] TemplateSection section, [NotNull] XElement fieldElement)
    {
      var fieldModel = new TemplateField();
      section.Fields.Add(fieldModel);

      fieldModel.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldModel.Name))
      {
        throw new BuildException(Texts.Text2008, sourceFile.SourceFileName, fieldElement);
      }

      fieldModel.Shared = string.Compare(fieldElement.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      fieldModel.Unversioned = string.Compare(fieldElement.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      fieldModel.Source = fieldElement.GetAttributeValue("Source") ?? string.Empty;

      fieldModel.Type = fieldElement.GetAttributeValue("Type");
      if (string.IsNullOrEmpty(fieldModel.Type))
      {
        fieldModel.Type = "Single-Line Text";
      }
    }

    protected void ParseSection([NotNull] IParseContext context, [NotNull] ISourceFile sourceFile, [NotNull] Template template, [NotNull] XElement sectionElement)
    {
      var sectionModel = new TemplateSection();
      template.Sections.Add(sectionModel);

      sectionModel.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, sourceFile.SourceFileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, sourceFile, sectionModel, fieldElement);
      }
    }
  }
}
