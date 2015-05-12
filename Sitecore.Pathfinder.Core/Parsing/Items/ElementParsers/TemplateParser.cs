namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TreeNodes;

  [Export(typeof(IElementParser))]
  public class TemplateParser : ElementParserBase
  {
    public override bool CanParse(ItemParseContext context, ITreeNode treeNode)
    {
      return treeNode.Name == "Template";
    }

    public override void Parse(ItemParseContext context, ITreeNode treeNode)
    {
      var template = new Template(context.ParseContext.Project, treeNode.TextSpan);
      context.ParseContext.Project.Items.Add(template);

      template.ItemName = treeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        template.ItemName = context.ParseContext.ItemName;
      }

      template.DatabaseName = context.ParseContext.DatabaseName;
      template.ItemIdOrPath = context.ParentItemPath + "/" + template.ItemName;
      template.BaseTemplates = treeNode.GetAttributeValue("BaseTemplates");
      template.Icon = treeNode.GetAttributeValue("Icon");

      foreach (var sectionTreeNode in treeNode.TreeNodes)
      {
        this.ParseSection(context, template, sectionTreeNode);
      }
    }

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] TemplateSection templateSection, [NotNull] ITreeNode fieldTreeNode)
    {
      var templateField = new TemplateField();
      templateSection.Fields.Add(templateField);

      templateField.Name = fieldTreeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateField.Name))
      {
        throw new BuildException(Texts.Text2008, fieldTreeNode.TextSpan);
      }

      templateField.Shared = fieldTreeNode.GetAttributeValue("Sharing") == "Shared";
      templateField.Unversioned = fieldTreeNode.GetAttributeValue("Sharing") == "Unversioned";
      templateField.Source = fieldTreeNode.GetAttributeValue("Source");

      templateField.Type = fieldTreeNode.GetAttributeValue("Type");
      if (string.IsNullOrEmpty(templateField.Type))
      {
        templateField.Type = "Single-Line Text";
      }
    }

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITreeNode sectionTreeNode)
    {
      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);

      templateSection.Name = sectionTreeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, sectionTreeNode.TextSpan);
      }

      foreach (var fieldTreeNode in sectionTreeNode.TreeNodes)
      {
        this.ParseField(context, templateSection, fieldTreeNode);
      }
    }
  }
}