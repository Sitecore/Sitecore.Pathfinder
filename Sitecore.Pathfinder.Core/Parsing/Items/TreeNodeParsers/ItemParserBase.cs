namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class ItemParserBase : TextNodeParserBase
  {
    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemName = textNode.GetAttributeValue("Name", context.ParseContext.ItemName);
      var itemIdOrPath = context.ParentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var item = new Item(context.ParseContext.Project, projectUniqueId, textNode)
      {
        ItemName = itemName,
        DatabaseName = context.ParseContext.DatabaseName,
        ItemIdOrPath = itemIdOrPath,
        TemplateIdOrPath = this.GetTemplateIdOrPath(context, textNode)
      };

      if (!string.IsNullOrEmpty(textNode.GetAttributeValue("Template.Create")))
      {
        var template = this.ParseTemplate(context, textNode);
        item.TemplateIdOrPath = template.ItemIdOrPath;
      }

      item.References.AddRange(this.ParseReferences(item, textNode, item.TemplateIdOrPath));

      this.ParseChildNodes(context, item, textNode);

      context.ParseContext.Project.AddOrMerge(item);
    }

    [CanBeNull]
    protected abstract ITextNode GetFieldTreeNode([NotNull] ITextNode textNode);

    [NotNull]
    protected virtual string GetTemplateIdOrPath([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
    {
      var templateIdOrPath = textNode.GetAttributeValue("Template");
      if (string.IsNullOrEmpty(templateIdOrPath))
      {
        templateIdOrPath = textNode.GetAttributeValue("Template.Create");
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
      return PathHelper.NormalizeItemPath(PathHelper.Combine(context.ParseContext.ItemPath, templateIdOrPath));
    }

    protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
    {
      var fieldName = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode.Document.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      var field = item.Fields.FirstOrDefault(f => string.Compare(f.Name, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode.Document.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      var language = fieldTextNode.GetAttributeValue("Language");

      int version = 0;
      var versionValue = fieldTextNode.GetAttributeValue("Version");
      if (!string.IsNullOrEmpty(versionValue))
      {
        if (!int.TryParse(versionValue, out version))
        {
          context.ParseContext.Trace.TraceError(Texts._version__attribute_must_have_an_integer_value, fieldTextNode.Document.SourceFile.FileName, fieldTextNode.Position, fieldName);
          version = 0;
        }
      }

      var valueTextNode = fieldTextNode.GetAttribute("[Value]");

      var valueAttributeTextNode = fieldTextNode.GetAttribute("Value");
      if (valueAttributeTextNode != null)
      {
        if (valueTextNode != null)
        {
          context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Document.SourceFile.FileName, valueAttributeTextNode.Position, fieldName);
        }

        valueTextNode = valueAttributeTextNode;
      }

      field = new Field(fieldTextNode, valueTextNode);
      item.Fields.Add(field);

      field.Name = fieldName;
      field.Language = language;
      field.Version = version;

      if (valueTextNode != null)
      {
        field.Value = valueTextNode.Value;
        item.References.AddRange(this.ParseReferences(item, valueTextNode, field.Value));
      }
    }

    [NotNull]
    protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] ITextNode textNode)
    {
      var itemIdOrPath = this.GetTemplateIdOrPath(context, textNode);
      if (string.IsNullOrEmpty(itemIdOrPath))
      {
        context.ParseContext.Trace.TraceError(Texts._Item__element_must_have_a__Template__or__Template_Create__attribute, textNode.Document.SourceFile.FileName, textNode.Position);
      }

      var n = itemIdOrPath.LastIndexOf('/');
      var itemName = itemIdOrPath.Mid(n + 1);
      var projectUniqueId = textNode.GetAttributeValue("Template.Id", itemIdOrPath);

      var template = new Template(context.ParseContext.Project, projectUniqueId, textNode)
      {
        ItemName = itemName,
        DatabaseName = context.ParseContext.DatabaseName,
        ItemIdOrPath = itemIdOrPath,
        Icon = textNode.GetAttributeValue("Template.Icon"),
        BaseTemplates = textNode.GetAttributeValue("Template.BaseTemplates", Constants.Templates.StandardTemplate),
        ShortHelp = textNode.GetAttributeValue("Template.ShortHelp"),
        LongHelp = textNode.GetAttributeValue("Template.LongHelp")
      };

      template.References.AddRange(this.ParseReferences(template, textNode, template.BaseTemplates));

      var templateSection = new TemplateSection();
      template.Sections.Add(templateSection);
      templateSection.Name = "Fields";
      templateSection.Icon = "Applications/16x16/form_blue.png";

      var fieldTreeNodes = this.GetFieldTreeNode(textNode);
      if (fieldTreeNodes != null)
      {
        foreach (var child in fieldTreeNodes.ChildNodes)
        {
          var name = child.GetAttributeValue("Name");

          var templateField = new TemplateField();
          templateSection.Fields.Add(templateField);
          templateField.Name = name;
          templateField.Type = child.GetAttributeValue("Field.Type", "Single-Line Text");
          templateField.Shared = string.Compare(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
          templateField.Unversioned = string.Compare(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
          templateField.Source = child.GetAttributeValue("Field.Source");
          templateField.ShortHelp = child.GetAttributeValue("Field.ShortHelp");
          templateField.LongHelp = child.GetAttributeValue("Field.LongHelp");

          template.References.AddRange(this.ParseReferences(template, child, templateField.Source));
        }
      }

      return context.ParseContext.Project.AddOrMerge(template);
    }

    protected abstract void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);
  }
}
