namespace Sitecore.Pathfinder.Parsing.Other
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
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
      return context.Document.SourceFile.SourceFileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    public override void Parse(IParseContext context)
    {
      var root = context.Document.Root;

      var privateTemplate = this.CreatePrivateTemplate(context, root);
      if (privateTemplate == null)
      {
        throw new BuildException(Texts.Text2031);
      }

      var publicTemplate = this.CreatePublicTemplate(context, privateTemplate);

      var component = new Component(context.Project, root, privateTemplate, publicTemplate);
      context.Project.Items.Add(component);
    }

    [NotNull]
    protected Template CreatePrivateTemplate([NotNull] IParseContext context, [NotNull] ITreeNode root)
    {
      var privateTemplate = new Template(context.Project, root);
      context.Project.Items.Add(privateTemplate);

      // todo: remove duplicated code from TemplateParser
      this.Parse(context, privateTemplate, root);

      privateTemplate.ItemName = "__" + privateTemplate.ItemName;
      var n = privateTemplate.ItemIdOrPath.LastIndexOf('/');
      privateTemplate.ItemIdOrPath = privateTemplate.ItemIdOrPath.Left(n + 1) + privateTemplate.ItemName;

      return privateTemplate;
    }

    [NotNull]
    protected Template CreatePublicTemplate([NotNull] IParseContext context, [NotNull] Template privateTemplate)
    {
      var publicTemplate = new Template(context.Project, privateTemplate.TreeNode);
      context.Project.Items.Add(publicTemplate);

      publicTemplate.ItemName = privateTemplate.ItemName.Mid(2);
      publicTemplate.DatabaseName = privateTemplate.DatabaseName;
      publicTemplate.ItemIdOrPath = PathHelper.NormalizeItemPath(Path.GetDirectoryName(privateTemplate.ItemIdOrPath) ?? string.Empty) + "/" + publicTemplate.ItemName;
      publicTemplate.BaseTemplates = privateTemplate.ItemIdOrPath;

      return publicTemplate;
    }

    protected void Parse([NotNull] IParseContext context, [NotNull] Template template, [NotNull] ITreeNode treeNode)
    {
      template.ItemName = treeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        template.ItemName = context.ItemName;
      }

      template.DatabaseName = context.DatabaseName;
      template.ItemIdOrPath = context.ItemPath;
      template.BaseTemplates = treeNode.GetAttributeValue("BaseTemplates");
      template.Icon = treeNode.GetAttributeValue("Icon");
      template.ShortHelp = treeNode.GetAttributeValue("ShortHelp");
      template.LongHelp = treeNode.GetAttributeValue("LongHelp");

      foreach (var sectionTreeNode in treeNode.TreeNodes)
      {
        this.ParseSection(context, template, sectionTreeNode);
      }
    }

    protected void ParseField([NotNull] IParseContext context, [NotNull] TemplateSection section, [NotNull] ITreeNode fieldTreeNode)
    {
      var templateField = new TemplateField();
      section.Fields.Add(templateField);

      templateField.Name = fieldTreeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateField.Name))
      {
        throw new BuildException(Texts.Text2008, fieldTreeNode);
      }

      templateField.Type = fieldTreeNode.GetAttributeValue("Type");
      templateField.Shared = string.Compare(fieldTreeNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTreeNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTreeNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTreeNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTreeNode.GetAttributeValue("LongHelp");
      templateField.StandardValue = fieldTreeNode.GetAttributeValue("StandardValue");

      if (string.IsNullOrEmpty(templateField.Type))
      {
        templateField.Type = "Single-Line Text";
      }
    }

    protected void ParseSection([NotNull] IParseContext context, [NotNull] Template template, [NotNull] ITreeNode sectionTreeNode)
    {
      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);

      templateSection.Name = sectionTreeNode.GetAttributeValue("Name");
      templateSection.Icon = sectionTreeNode.GetAttributeValue("Icon");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, sectionTreeNode);
      }

      foreach (var fieldTreeNode in sectionTreeNode.TreeNodes)
      {
        this.ParseField(context, templateSection, fieldTreeNode);
      }
    }
  }
}
