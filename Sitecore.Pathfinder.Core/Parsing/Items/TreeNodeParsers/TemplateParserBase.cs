namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Templates;

  public abstract class TemplateParserBase : TextNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = context.ParentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var template = new Template(context.ParseContext.Project, projectUniqueId, textNode)
      {
        ItemName = itemName,
        DatabaseName = context.ParseContext.DatabaseName,
        ItemIdOrPath = itemIdOrPath,
        BaseTemplates = textNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate),
        Icon = textNode.GetAttributeValue("Icon"),
        ShortHelp = textNode.GetAttributeValue("ShortHelp"),
        LongHelp = textNode.GetAttributeValue("LongHelp")
      };

      template.References.AddRange(this.ParseReferences(template, textNode, template.BaseTemplates));

      var sectionsTextNode = this.GetSectionsTextNode(textNode);
      if (sectionsTextNode != null)
      {
        foreach (var sectionTreeNode in sectionsTextNode.ChildNodes)
        {
          this.ParseSection(context, template, sectionTreeNode);
        }
      }

      context.ParseContext.Project.AddOrMerge(template);
    }

    [CanBeNull]
    protected abstract ITextNode GetFieldsTextNode([NotNull] ITextNode textNode);

    [CanBeNull]
    protected abstract ITextNode GetSectionsTextNode([NotNull] ITextNode textNode);

    protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode fieldTextNode)
    {
      var fieldName = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode.DocumentSnapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      var templateField = templateSection.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (templateField == null)
      {
        templateField = new TemplateField();
        templateSection.Fields.Add(templateField);
        templateField.Name = fieldName;
      }

      templateField.Type = fieldTextNode.GetAttributeValue("Type", "Single-Line Text");
      templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
      templateField.Source = fieldTextNode.GetAttributeValue("Source");
      templateField.ShortHelp = fieldTextNode.GetAttributeValue("ShortHelp");
      templateField.LongHelp = fieldTextNode.GetAttributeValue("LongHelp");
      templateField.StandardValue = fieldTextNode.GetAttributeValue("StandardValue");

      template.References.AddRange(this.ParseReferences(template, fieldTextNode, templateField.Source));
    }

    protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode sectionTextNode)
    {
      var sectionName = sectionTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(sectionName))
      {
        context.ParseContext.Trace.TraceError(Texts._Section__element_must_have_a__Name__attribute, sectionTextNode.DocumentSnapshot.SourceFile.FileName, sectionTextNode.Position);
      }

      var templateSection = template.Sections.FirstOrDefault(s => string.Compare(s.Name, sectionName, StringComparison.OrdinalIgnoreCase) == 0);
      if (templateSection == null)
      {
        templateSection = new TemplateSection();
        template.Sections.Add(templateSection);
        templateSection.Name = sectionName;
      }

      templateSection.Icon = sectionTextNode.GetAttributeValue("Icon");

      var fieldsTextNode = this.GetFieldsTextNode(sectionTextNode);
      if (fieldsTextNode == null)
      {
        return;
      }

      foreach (var fieldTextNode in fieldsTextNode.ChildNodes)
      {
        this.ParseField(context, template, templateSection, fieldTextNode);
      }
    }
  }
}