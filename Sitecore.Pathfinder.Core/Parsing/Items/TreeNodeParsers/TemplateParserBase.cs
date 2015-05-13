namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Templates;

  public abstract class TemplateParserBase : TreeNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITreeNode treeNode)
    {
      var template = new Template(context.ParseContext.Project, treeNode);

      var itemName = treeNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = context.ParentItemPath + "/" + template.ItemName;
      var projectId = treeNode.GetAttributeValue("Id", "{" + itemIdOrPath + "}");

      template.ProjectId = projectId;
      template.ItemName = itemName;
      template.DatabaseName = context.ParseContext.DatabaseName;
      template.ItemIdOrPath = itemIdOrPath;
      template.BaseTemplates = treeNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate);
      template.Icon = treeNode.GetAttributeValue("Icon");
      template.ShortHelp = treeNode.GetAttributeValue("ShortHelp");
      template.LongHelp = treeNode.GetAttributeValue("LongHelp");

      var sectionsTreeNode = this.GetSectionsTreeNode(treeNode);
      if (sectionsTreeNode != null)
      {
        foreach (var sectionTreeNode in sectionsTreeNode.TreeNodes)
        {
          this.ParseSection(context, template, sectionTreeNode);
        }
      }

      context.ParseContext.Project.Items.Add(template);
    }

    [CanBeNull]
    protected abstract ITreeNode GetFieldsTreeNode([NotNull] ITreeNode treeNode);

    [CanBeNull]
    protected abstract ITreeNode GetSectionsTreeNode([NotNull] ITreeNode treeNode);

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] TemplateSection templateSection, [NotNull] ITreeNode fieldTreeNode)
    {
      var templateField = new TemplateField();
      templateSection.Fields.Add(templateField);

      templateField.Name = fieldTreeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(templateField.Name))
      {
        throw new BuildException(Texts.Text2008, fieldTreeNode);
      }

      templateField.Type = fieldTreeNode.GetAttributeValue("Type", "Single-Line Text");
      templateField.Shared = string.Compare(fieldTreeNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTreeNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTreeNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTreeNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTreeNode.GetAttributeValue("LongHelp");
      templateField.StandardValue = fieldTreeNode.GetAttributeValue("StandardValue");
    }

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITreeNode sectionTreeNode)
    {
      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);

      templateSection.Name = sectionTreeNode.GetAttributeValue("Name");
      templateSection.Icon = sectionTreeNode.GetAttributeValue("Icon");

      if (string.IsNullOrEmpty(template.ItemName))
      {
        throw new BuildException(Texts.Text2007, sectionTreeNode);
      }

      var fieldsTreeNode = this.GetFieldsTreeNode(sectionTreeNode);
      if (fieldsTreeNode == null)
      {
        return;
      }

      foreach (var fieldTreeNode in fieldsTreeNode.TreeNodes)
      {
        this.ParseField(context, templateSection, fieldTreeNode);
      }
    }
  }
}
