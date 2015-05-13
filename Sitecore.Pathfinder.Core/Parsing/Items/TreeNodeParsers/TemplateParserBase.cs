namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Templates;

  public abstract class TemplateParserBase : TreeNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITreeNode treeNode)
    {
      var template = new Template(context.ParseContext.Project, treeNode);

      template.ItemName = treeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(template.ItemName))
      {
        template.ItemName = context.ParseContext.ItemName;
      }

      template.DatabaseName = context.ParseContext.DatabaseName;
      template.ItemIdOrPath = context.ParentItemPath + "/" + template.ItemName;
      template.BaseTemplates = treeNode.GetAttributeValue("BaseTemplates");
      template.Icon = treeNode.GetAttributeValue("Icon");

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

      templateField.Shared = string.Compare(fieldTreeNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTreeNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
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
