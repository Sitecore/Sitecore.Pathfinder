namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;

  public abstract class ItemParserBase : TreeNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITreeNode treeNode)
    {
      var item = new Item(context.ParseContext.Project, treeNode);

      item.ItemName = treeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(item.ItemName))
      {
        item.ItemName = context.ParseContext.ItemName;
      }

      item.DatabaseName = context.ParseContext.DatabaseName;
      item.ItemIdOrPath = context.ParentItemPath + "/" + item.ItemName;
      item.TemplateIdOrPath = this.GetTemplateIdOrPath(context, treeNode);

      if (!string.IsNullOrEmpty(treeNode.GetAttributeValue("Template.Create")))
      {
        var template = this.ParseTemplate(context, treeNode);
        item.TemplateIdOrPath = template.ItemIdOrPath;
      }

      context.ParseContext.Project.Items.Add(item);

      this.ParseTreeNodes(context, item, treeNode);
    }

    [CanBeNull]
    protected abstract ITreeNode GetFieldTreeNode([NotNull] ITreeNode treeNode);

    [NotNull]
    protected virtual string GetTemplateIdOrPath([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      var templateIdOrPath = treeNode.GetAttributeValue("Template");
      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        templateIdOrPath = treeNode.GetAttributeValue("Template.Create");
      }

      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        return string.Empty;
      }

      // return if absolute path or guid
      templateIdOrPath = templateIdOrPath.Trim();
      if (templateIdOrPath.StartsWith("/") || templateIdOrPath.StartsWith("{"))
      {
        return templateIdOrPath;
      }

      // resolve relative paths
      templateIdOrPath = PathHelper.NormalizeItemPath(PathHelper.Combine(context.ParseContext.ItemPath, templateIdOrPath));

      return templateIdOrPath;
    }

    protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITreeNode fieldTreeNode)
    {
      var fieldName = fieldTreeNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        throw new BuildException(Texts.Text2011, fieldTreeNode);
      }

      var field = item.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text2012, fieldTreeNode.Document.SourceFile.SourceFileName, fieldTreeNode.LineNumber, fieldTreeNode.LinePosition, fieldName);
        return;
      }

      var treeNodeValue = fieldTreeNode.Value;
      var attributeValue = fieldTreeNode.GetAttributeValue("Value");
      if (!string.IsNullOrEmpty(treeNodeValue) && !string.IsNullOrEmpty(attributeValue))
      {
        context.ParseContext.Project.Trace.TraceWarning(Texts.Text3027, fieldTreeNode.Document.SourceFile.SourceFileName, fieldTreeNode.LineNumber, fieldTreeNode.LinePosition, fieldName);
      }

      var value = !string.IsNullOrEmpty(attributeValue) ? attributeValue : treeNodeValue;

      field = new Field(item.TreeNode);
      item.Fields.Add(field);

      field.Name = fieldName;
      field.Value = value;
    }

    [NotNull]
    protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      var template = new Template(context.ParseContext.Project, treeNode);

      template.ItemIdOrPath = this.GetTemplateIdOrPath(context, treeNode);
      if (string.IsNullOrEmpty(template.ItemIdOrPath))
      {
        throw new BuildException(Texts.Text2010, treeNode);
      }

      template.DatabaseName = context.ParseContext.DatabaseName;
      template.Icon = treeNode.GetAttributeValue("Template.Icon");
      template.BaseTemplates = treeNode.GetAttributeValue("Template.BaseTemplates");
      template.ShortHelp = treeNode.GetAttributeValue("Template.ShortHelp");
      template.LongHelp = treeNode.GetAttributeValue("Template.LongHelp");

      if (string.IsNullOrEmpty(template.BaseTemplates))
      {
        template.BaseTemplates = Constants.Templates.StandardTemplate;
      }

      // get template name
      var n = template.ItemIdOrPath.LastIndexOf('/');
      template.ItemName = template.ItemIdOrPath.Mid(n + 1);

      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);
      templateSection.Name = "Fields";
      templateSection.Icon = "Applications/16x16/form_blue.png";

      var fieldTreeNodes = this.GetFieldTreeNode(treeNode);
      if (fieldTreeNodes != null)
      {
        foreach (var child in fieldTreeNodes.TreeNodes)
        {
          var name = child.GetAttributeValue("Name");

          var templateField = new TemplateField();
          templateSection.Fields.Add(templateField);
          templateField.Name = name;
          templateField.Type = child.GetAttributeValue("Field.Type");
          templateField.Shared = string.Compare(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
          templateField.Unversioned = string.Compare(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
          templateField.Source = child.GetAttributeValue("Field.Source");
          templateField.ShortHelp = child.GetAttributeValue("Field.ShortHelp");
          templateField.LongHelp = child.GetAttributeValue("Field.LongHelp");

          if (string.IsNullOrEmpty(templateField.Type))
          {
            templateField.Type = "Single-Line Text";
          }
        }
      }

      context.ParseContext.Project.Items.Add(template);
      return template;
    }

    protected abstract void ParseTreeNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITreeNode treeNode);
  }
}
