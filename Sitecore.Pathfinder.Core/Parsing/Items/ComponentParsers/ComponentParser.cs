namespace Sitecore.Pathfinder.Parsing.Items.ComponentParsers
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Extensions.XElementExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Models.Templates;

  [Export(typeof(IItemParser))]
  public class ComponentParser : ItemParserBase
  {
    private const string FileExtension = ".component.xml";

    public ComponentParser() : base(Template)
    {
    }

    public override bool CanParse(IItemParseContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IItemParseContext context)
    {
      var root = this.LoadXmlFile(context);

      var templateModel = this.CreatePrivateTemplate(context, root);
      if (templateModel == null)
      {
        throw new BuildException(Texts.Text2031);
      }

      this.CreatePublicTemplate(context, templateModel);
    }

    public void Parse([NotNull] IItemParseContext context, [NotNull] TemplateModel templateModel, [NotNull] XElement element)
    {
      templateModel.Name = element.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        templateModel.Name = context.ItemName;
      }

      templateModel.DatabaseName = context.DatabaseName;
      templateModel.ItemIdOrPath = context.ItemPath;
      templateModel.BaseTemplates = element.GetAttributeValue("BaseTemplates") ?? string.Empty;
      templateModel.Icon = element.GetAttributeValue("Icon") ?? string.Empty;

      foreach (var sectionElement in element.Elements())
      {
        this.ParseSection(context, templateModel, sectionElement);
      }
    }

    [CanBeNull]
    protected TemplateModel CreatePrivateTemplate([NotNull] IItemParseContext context, [NotNull] XElement root)
    {
      var templateModel = new TemplateModel(context.FileName);
      context.ParseContext.Project.Models.Add(templateModel);

      // todo: remove duplicated code from TemplateParser
      this.Parse(context, templateModel, root);

      templateModel.Name = "__" + templateModel.Name;
      var n = templateModel.ItemIdOrPath.LastIndexOf('/');
      templateModel.ItemIdOrPath = templateModel.ItemIdOrPath.Left(n + 1) + templateModel.Name;

      return templateModel;
    }

    protected void CreatePublicTemplate([NotNull] IItemParseContext context, [NotNull] TemplateModel privateTemplate)
    {
      var templateModel = new TemplateModel(context.FileName);
      context.ParseContext.Project.Models.Add(templateModel);

      templateModel.Name = privateTemplate.Name.Mid(2);
      templateModel.DatabaseName = privateTemplate.DatabaseName;
      templateModel.ItemIdOrPath = PathHelper.NormalizeWebPath(Path.GetDirectoryName(privateTemplate.ItemIdOrPath)) + "/" + templateModel.Name;
      templateModel.BaseTemplates = privateTemplate.ItemIdOrPath;
    }

    private void ParseField([NotNull] IItemParseContext context, [NotNull] TemplateSectionModel sectionModel, [NotNull] XElement fieldElement)
    {
      var fieldModel = new TemplateFieldModel();
      sectionModel.Fields.Add(fieldModel);

      fieldModel.Name = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldModel.Name))
      {
        throw new BuildException(Texts.Text2008, context.FileName, fieldElement);
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

    private void ParseSection([NotNull] IItemParseContext context, [NotNull] TemplateModel templateModel, [NotNull] XElement sectionElement)
    {
      var sectionModel = new TemplateSectionModel();
      templateModel.Sections.Add(sectionModel);

      sectionModel.Name = sectionElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateModel.Name))
      {
        throw new BuildException(Texts.Text2007, context.FileName, sectionElement);
      }

      foreach (var fieldElement in sectionElement.Elements())
      {
        this.ParseField(context, sectionModel, fieldElement);
      }
    }
  }
}
