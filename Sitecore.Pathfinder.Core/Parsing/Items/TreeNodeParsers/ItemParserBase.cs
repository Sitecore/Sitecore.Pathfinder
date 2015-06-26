// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing.Pipelines.ItemParserPipelines;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    public abstract class ItemParserBase : TextNodeParserBase
    {
        protected ItemParserBase(double priority) : base(priority)
        {
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = textNode.GetAttributeTextNode("Name");
            var itemName = itemNameTextNode?.Value ?? context.ParseContext.ItemName;

            var itemIdOrPath = context.ParentItemPath + "/" + itemName;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            // todo: consider moving template into the ItemParserPipeline
            var templateIdOrPathTextNode = textNode.GetAttributeTextNode("Template");
            if (templateIdOrPathTextNode == null)
            {
                templateIdOrPathTextNode = textNode.GetAttributeTextNode("Template.Create");
                if (templateIdOrPathTextNode != null)
                {
                    ParseTemplate(context, textNode, templateIdOrPathTextNode);
                }
            }

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath, templateIdOrPathTextNode?.Value ?? string.Empty);
            item.ItemName.Source = itemNameTextNode ?? new FileNameTextNode(item.ItemName.Value, textNode.Snapshot);
            item.TemplateIdOrPath.Source = templateIdOrPathTextNode;

            if (!string.IsNullOrEmpty(item.TemplateIdOrPath.Value))
            {
                var a = textNode.GetAttributeTextNode("Template") ?? textNode.GetAttributeTextNode("Template.Create");
                if (a != null)
                {
                    item.References.AddRange(ParseReferences(context, item, a, item.TemplateIdOrPath.Value));
                }
            }

            context.ParseContext.PipelineService.Resolve<ItemParserPipeline>().Execute(context, item, textNode);

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
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

        protected abstract void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

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

            var nameTextNode = fieldTextNode.GetAttributeTextNode("Name") ?? fieldTextNode;
            var valueTextNode = fieldTextNode.GetAttributeTextNode("[Value]");

            var valueAttributeTextNode = fieldTextNode.GetAttributeTextNode("Value");
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
                item.References.AddRange(ParseReferences(context, item, valueTextNode, field.Value.Value));
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
            var icon = itemTextNode.GetAttributeTextNode("Template.Icon");
            template.Icon.SetValue(icon != null ? new Attribute<string>(icon) : new Attribute<string>("Template.Icon", string.Empty));
            template.BaseTemplates = itemTextNode.GetAttributeValue("Template.BaseTemplates", Constants.Templates.StandardTemplate);
            template.ShortHelp = itemTextNode.GetAttributeValue("Template.ShortHelp");
            template.LongHelp = itemTextNode.GetAttributeValue("Template.LongHelp");

            template.References.AddRange(ParseReferences(context, template, itemTextNode, template.BaseTemplates));

            var templateSection = context.ParseContext.Factory.TemplateSection(TextNode.Empty);
            template.Sections.Add(templateSection);
            templateSection.SectionName.SetValue("Fields");
            templateSection.Icon = "Applications/16x16/form_blue.png";

            var fieldTreeNodes = context.Snapshot.GetJsonChildTextNode(itemTextNode, "Fields");
            if (fieldTreeNodes != null)
            {
                var nextSortOrder = 0;
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
                    templateField.FieldName.SetValue(child.GetAttributeTextNode("Name"));
                    templateField.Type = child.GetAttributeValue("Field.Type", "Single-Line Text");
                    templateField.Shared = string.Compare(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
                    templateField.Unversioned = string.Compare(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
                    templateField.Source = child.GetAttributeValue("Field.Source");
                    templateField.ShortHelp = child.GetAttributeValue("Field.ShortHelp");
                    templateField.LongHelp = child.GetAttributeValue("Field.LongHelp");
                    templateField.SortOrder = sortOrder;

                    template.References.AddRange(ParseReferences(context, template, child, templateField.Source));
                }
            }

            return context.ParseContext.Project.AddOrMerge(context.ParseContext, template);
        }
    }
}
