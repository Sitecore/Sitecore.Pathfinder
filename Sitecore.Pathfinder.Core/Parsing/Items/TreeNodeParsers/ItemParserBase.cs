namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;

  public abstract class ItemParserBase : TextNodeParserBase
  {
    protected ItemParserBase(double priority) : base(priority)
    {
    }

    public override void Parse(ItemParseContext context, ITextNode textNode)
    {
      var itemNameTextNode = textNode.GetTextNodeAttribute("Name");
      var itemName = itemNameTextNode?.Value ?? context.ParseContext.ItemName;
      var itemIdOrPath = context.ParentItemPath + "/" + itemName;
      var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

      var templateIdOrPathTextNode = textNode.GetTextNodeAttribute("Template");
      if (templateIdOrPathTextNode == null)
      {
        templateIdOrPathTextNode = textNode.GetTextNodeAttribute("Template.Create");
        if (templateIdOrPathTextNode != null)
        {
          this.ParseTemplate(context, textNode, templateIdOrPathTextNode);
        }
      }

      var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath, templateIdOrPathTextNode?.Value ?? string.Empty);
      item.ItemName.Source = itemNameTextNode;

      if (!string.IsNullOrEmpty(item.TemplateIdOrPath.Value))
      {
        var a = textNode.GetTextNodeAttribute("Template") ?? textNode.GetTextNodeAttribute("Template.Create");
        if (a != null)
        {
          item.References.AddRange(this.ParseReferences(context, item, a, item.TemplateIdOrPath.Value));
        }
      }

      this.ParseChildNodes(context, item, textNode);

      context.ParseContext.Project.AddOrMerge(item);
    }

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

      templateIdOrPath = templateIdOrPath.Trim();

      // resolve relative paths
      if (!templateIdOrPath.StartsWith("/") && !templateIdOrPath.StartsWith("{"))
      {
        templateIdOrPath = PathHelper.NormalizeItemPath(PathHelper.Combine(context.ParseContext.ItemPath, templateIdOrPath));
      }

      return templateIdOrPath;
    }

    protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
    {
      var fieldName = fieldTextNode.GetAttributeValue("Name");
      if (string.IsNullOrEmpty(fieldName))
      {
        context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
      if (field != null)
      {
        context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
      }

      var valueHint = fieldTextNode.GetAttributeValue("Value.Hint");
      var language = fieldTextNode.GetAttributeValue("Language");

      var version = 0;
      var versionValue = fieldTextNode.GetAttributeValue("Version");
      if (!string.IsNullOrEmpty(versionValue))
      {
        if (!int.TryParse(versionValue, out version))
        {
          context.ParseContext.Trace.TraceError(Texts._version__attribute_must_have_an_integer_value, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position, fieldName);
          version = 0;
        }
      }

      var nameTextNode = fieldTextNode.GetTextNodeAttribute("Name") ?? fieldTextNode;
      var valueTextNode = fieldTextNode.GetTextNodeAttribute("[Value]");

      var valueAttributeTextNode = fieldTextNode.GetTextNodeAttribute("Value");
      if (valueAttributeTextNode != null)
      {
        if (valueTextNode != null)
        {
          context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Snapshot.SourceFile.FileName, valueAttributeTextNode.Position, fieldName);
        }

        valueTextNode = valueAttributeTextNode;
      }

      if (valueTextNode == null)
      {
        valueTextNode = context.ParseContext.Factory.TextNode(fieldTextNode.Snapshot, TextPosition.Empty, "Value", string.Empty, null);
      }

      field = context.ParseContext.Factory.Field(item, fieldName, language, version, valueTextNode.Value, valueHint);
      field.FieldName.Source = nameTextNode;
      field.Value.Source = valueTextNode;
      item.Fields.Add(field);

      if (field.ValueHint != "Text")
      {
        item.References.AddRange(this.ParseReferences(context, item, valueTextNode, field.Value.Value));
      }
    }

    [NotNull]
    protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] ITextNode itemTextNode, [NotNull] ITextNode templateIdOrPathTextNode)
    {
      var itemIdOrPath = templateIdOrPathTextNode.Value;

      var n = itemIdOrPath.LastIndexOf('/');
      var itemName = itemIdOrPath.Mid(n + 1);
      var projectUniqueId = itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath);

      var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, itemTextNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath);
      template.ItemName.Source = templateIdOrPathTextNode;
      template.Icon = itemTextNode.GetAttributeValue("Template.Icon");
      template.BaseTemplates = itemTextNode.GetAttributeValue("Template.BaseTemplates", Constants.Templates.StandardTemplate);
      template.ShortHelp = itemTextNode.GetAttributeValue("Template.ShortHelp");
      template.LongHelp = itemTextNode.GetAttributeValue("Template.LongHelp");

      template.References.AddRange(this.ParseReferences(context, template, itemTextNode, template.BaseTemplates));

      var templateSection = context.ParseContext.Factory.TemplateSection();
      template.Sections.Add(templateSection);
      templateSection.Name = "Fields";
      templateSection.Icon = "Applications/16x16/form_blue.png";

      var fieldTreeNodes = context.Snapshot.GetJsonChildTextNode(itemTextNode, "Fields");
      if (fieldTreeNodes != null)
      {
        int nextSortOrder = 0;
        foreach (var child in fieldTreeNodes.ChildNodes)
        {
          if (child.Name != string.Empty && child.Name != "Field")
          {
            continue;
          }

          int sortOrder;
          if (!int.TryParse(child.GetAttributeValue("Field.SortOrder"), out sortOrder))
          {
            sortOrder = nextSortOrder;
          }

          nextSortOrder = sortOrder + 100;

          var templateField = context.ParseContext.Factory.TemplateField(template);
          templateSection.Fields.Add(templateField);
          templateField.Name = child.GetAttributeValue("Name");
          templateField.Type = child.GetAttributeValue("Field.Type", "Single-Line Text");
          templateField.Shared = string.Compare(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
          templateField.Unversioned = string.Compare(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
          templateField.Source = child.GetAttributeValue("Field.Source");
          templateField.ShortHelp = child.GetAttributeValue("Field.ShortHelp");
          templateField.LongHelp = child.GetAttributeValue("Field.LongHelp");
          templateField.SortOrder = sortOrder;

          template.References.AddRange(this.ParseReferences(context, template, child, templateField.Source));
        }
      }

      return context.ParseContext.Project.AddOrMerge(template);
    }

    protected abstract void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);
  }
}
