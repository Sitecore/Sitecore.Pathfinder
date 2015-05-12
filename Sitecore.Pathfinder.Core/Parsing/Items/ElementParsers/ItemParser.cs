namespace Sitecore.Pathfinder.Parsing.Items.ElementParsers
{
  using System;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TreeNodes;

  [Export(typeof(IElementParser))]
  public class ItemParser : ElementParserBase
  {
    public override bool CanParse(ItemParseContext context, ITreeNode treeNode)
    {
      return treeNode.Name == "Item";
    }

    public override void Parse(ItemParseContext context, ITreeNode treeNode)
    {
      var item = new Item(context.ParseContext.Project, treeNode.TextSpan);
      context.ParseContext.Project.Items.Add(item);

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

      this.ParseChildElements(context, item, treeNode);
    }

    [NotNull]
    protected virtual string GetTemplateIdOrPath([NotNull] ItemParseContext context, [NotNull] ITreeNode element)
    {
      var templateIdOrPath = element.GetAttributeValue("Template");
      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        templateIdOrPath = element.GetAttributeValue("Template.Create");
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

    protected virtual void ParseChildElements([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITreeNode element)
    {
      foreach (var child in element.TreeNodes)
      {
        if (child.Name == "Field")
        {
          this.ParseFieldElement(context, item, child);
        }
        else
        {
          var newContext = new ItemParseContext(context.ParseContext, context.Parser, context.ParentItemPath + "/" + child.Name);
          context.Parser.ParseTreeNode(newContext, child);
        }
      }
    }

    protected virtual void ParseFieldElement([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITreeNode fieldElement)
    {
      var fieldName = fieldElement.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        throw new BuildException(Texts.Text2011, fieldElement.TextSpan);
      }

      var field = item.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        context.ParseContext.Project.Trace.TraceError(Texts.Text2012, fieldElement.TextSpan, fieldName);
        return;
      }

      var value = fieldElement.GetAttributeValue("Value");

      field = new Field(item.TextSpan);
      item.Fields.Add(field);

      field.Name = fieldName;
      field.Value = value;
    }

    [NotNull]
    protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] ITreeNode treeNode)
    {
      var template = new Template(context.ParseContext.Project, treeNode.TextSpan);
      context.ParseContext.Project.Items.Add(template);

      template.ItemIdOrPath = this.GetTemplateIdOrPath(context, treeNode);
      if (string.IsNullOrEmpty(template.ItemIdOrPath))
      {
        throw new BuildException(Texts.Text2010, treeNode.TextSpan);
      }

      template.DatabaseName = context.ParseContext.DatabaseName;
      template.Icon = treeNode.GetAttributeValue("Template.Icon");
      template.BaseTemplates = treeNode.GetAttributeValue("Template.BaseTemplates");
      if (string.IsNullOrEmpty(template.BaseTemplates))
      {
        template.BaseTemplates = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";
      }

      // get template name
      var n = template.ItemIdOrPath.LastIndexOf('/');
      template.ItemName = template.ItemIdOrPath.Mid(n + 1);

      var sectionBuilder = new TemplateSection();
      template.Sections.Add(sectionBuilder);
      sectionBuilder.Name = "Fields";

      foreach (var child in treeNode.TreeNodes)
      {
        if (child.Name != "Field")
        {
          throw new BuildException(Texts.Text2015, child.TextSpan);
        }

        var name = child.GetAttributeValue("Name");

        var fieldModel = new TemplateField();
        sectionBuilder.Fields.Add(fieldModel);
        fieldModel.Name = name;
        fieldModel.Type = child.GetAttributeValue("Field.Type");
        if (string.IsNullOrEmpty(fieldModel.Type))
        {
          fieldModel.Type = "Single-Line Text";
        }

        fieldModel.Shared = child.GetAttributeValue("Field.Sharing") == "Shared";
        fieldModel.Unversioned = child.GetAttributeValue("Field.Sharing") == "Unversioned";
        fieldModel.Source = child.GetAttributeValue("Field.Source");
      }

      return template;
    }
  }
}