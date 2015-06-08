namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.ComponentModel.Composition;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Templates;

  [Export(typeof(ITextNodeParser))]
  public class TemplateParser : TextNodeParserBase
  {
    public TemplateParser() : base(Constants.TextNodeParsers.Templates)
    {
    }

    public override bool CanParse(ItemParseContext context, ITextNode textNode)
    {
      return textNode.Name == "Template";
    }

    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = context.ParentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath);
      template.BaseTemplates = textNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate);
      template.Icon = textNode.GetAttributeValue("Icon");
      template.ShortHelp = textNode.GetAttributeValue("ShortHelp");
      template.LongHelp = textNode.GetAttributeValue("LongHelp");

      template.References.AddRange(this.ParseReferences(context, template, textNode, template.BaseTemplates));

      var sectionsTextNode = context.Snapshot.GetJsonChildTextNode(textNode, "Sections");
      if (sectionsTextNode != null)
      {
        foreach (var sectionTreeNode in sectionsTextNode.ChildNodes)
        {
          this.ParseSection(context, template, sectionTreeNode);
        }
      }

      context.ParseContext.Project.AddOrMerge(template);
    }

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode fieldTextNode, ref int nextSortOrder)
    {
      var fieldName = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      var templateField = templateSection.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (templateField == null)
      {
        templateField = context.ParseContext.Factory.TemplateField(template);
        templateSection.Fields.Add(templateField);
        templateField.Name = fieldName;
      }

      int sortOrder;
      if (!int.TryParse(fieldTextNode.GetAttributeValue("SortOrder"), out sortOrder))
      {
        sortOrder = nextSortOrder;
      }

      nextSortOrder = sortOrder + 100;

      templateField.Type = fieldTextNode.GetAttributeValue("Type", "Single-Line Text");
      templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTextNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTextNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTextNode.GetAttributeValue("LongHelp");
      templateField.SortOrder = sortOrder;
      templateField.StandardValue = fieldTextNode.GetAttributeValue("StandardValue");

      template.References.AddRange(this.ParseReferences(context, template, fieldTextNode, templateField.Source));
    }

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode sectionTextNode)
    {
      var sectionName = sectionTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(sectionName))
      {
        context.ParseContext.Trace.TraceError(Texts._Section__element_must_have_a__Name__attribute, sectionTextNode.Snapshot.SourceFile.FileName, sectionTextNode.Position);
      }

      var templateSection = template.Sections.FirstOrDefault(s => string.Compare(s.Name, sectionName, StringComparison.OrdinalIgnoreCase) == 0);
      if (templateSection == null)
      {
        templateSection = context.ParseContext.Factory.TemplateSection();
        template.Sections.Add(templateSection);
        templateSection.Name = sectionName;
      }

      templateSection.Icon = sectionTextNode.GetAttributeValue("Icon");

      var fieldsTextNode = context.Snapshot.GetJsonChildTextNode(sectionTextNode, "Fields");
      if (fieldsTextNode == null)
      {
        return;
      }

      var nextSortOrder = 0;
      foreach (var fieldTextNode in fieldsTextNode.ChildNodes)
      {
        this.ParseField(context, template, templateSection, fieldTextNode, ref nextSortOrder);
      }
    }
  }
}