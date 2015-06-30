// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Extensions;
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
            var itemName = GetItemName(context.ParseContext, textNode);
            var itemIdOrPath = context.ParentItemPath + "/" + itemName.Value;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            // todo: consider moving template into the ItemParserPipeline
            var templateIdOrPath = textNode.GetAttributeTextNode("Template");
            if (templateIdOrPath == null)
            {
                templateIdOrPath = textNode.GetAttributeTextNode("Template.Create");
                if (templateIdOrPath != null)
                {
                    ParseTemplate(context, textNode, templateIdOrPath);
                }
            }

            var item = context.ParseContext.Factory.Item(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName.Value, itemIdOrPath, templateIdOrPath?.Value ?? string.Empty);
            item.ItemName.Merge(itemName);
            item.TemplateIdOrPath.AddSource(templateIdOrPath);

            if (templateIdOrPath != null)
            {
              item.References.AddRange(ParseReferences(context, item, templateIdOrPath, templateIdOrPath.Value));
            }

            context.ParseContext.PipelineService.Resolve<ItemParserPipeline>().Execute(context, item, textNode);

            ParseChildNodes(context, item, textNode);

            context.ParseContext.Project.AddOrMerge(context.ParseContext, item);
        }

        protected abstract void ParseChildNodes([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode textNode);

        protected virtual void ParseFieldTreeNode([NotNull] ItemParseContext context, [NotNull] Item item, [NotNull] ITextNode fieldTextNode)
        {
            var fieldName = fieldTextNode.GetAttributeTextNode("Name");
            if (fieldName == null || string.IsNullOrEmpty(fieldName.Value))
            {
                context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode);
                return;
            }

            // check if field is already defined
            var field = item.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, fieldName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (field != null)
            {
                context.ParseContext.Trace.TraceError(Texts.Field_is_already_defined, fieldTextNode, fieldName.Value);
            }

            // check if version is an integer
            var versionValue = fieldTextNode.GetAttributeValue("Version");
            if (!string.IsNullOrEmpty(versionValue))
            {
                int version;
                if (!int.TryParse(versionValue, out version))
                {
                    context.ParseContext.Trace.TraceError(Texts._version__attribute_must_have_an_integer_value, fieldTextNode, fieldName.Value);
                }
            }

            field = context.ParseContext.Factory.Field(item);
            field.FieldName.Parse(fieldTextNode);
            field.Language.Parse(fieldTextNode);
            field.Version.Parse(fieldTextNode);
            field.ValueHint.Parse(fieldTextNode);

            // get field value either from Value attribute or from inner text
            var inner = fieldTextNode.GetInnerTextNode();
            var attribute = fieldTextNode.GetAttributeTextNode("Value");

            if (inner != null && attribute != null)
            {
                context.ParseContext.Trace.TraceWarning(Texts.Value_is_specified_in_both__Value__attribute_and_in_element__Using_value_from_attribute, fieldTextNode.Snapshot.SourceFile.FileName, attribute.Position, fieldName.Value);
                field.Value.AddSource(attribute);
            }
            else if (inner != null)
            {
                field.Value.AddSource(inner);
            }
            else if (attribute != null)
            {
                field.Value.AddSource(attribute);
            }

            item.Fields.Add(field);

            if (field.Value.Source != null && !field.ValueHint.Value.Contains("NoReference"))
            {
                item.References.AddRange(ParseReferences(context, item, field.Value.Source, field.Value.Value));
            }
        }

        [NotNull]
        protected virtual Template ParseTemplate([NotNull] ItemParseContext context, [NotNull] ITextNode itemTextNode, [NotNull] ITextNode templateIdOrPathTextNode)
        {
            var itemIdOrPath = templateIdOrPathTextNode.Value;

            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);
            var projectUniqueId = itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath);

            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, itemTextNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath);

            template.ItemName.AddSource(templateIdOrPathTextNode);
            template.ItemName.SourceFlags = SourceFlags.IsQualified;
            template.ItemName.SetValue(itemName);

            template.Icon.Parse("Template.Icon", itemTextNode);
            template.BaseTemplates.Parse("Template.BaseTemplates", itemTextNode, Constants.Templates.StandardTemplate);
            template.ShortHelp.Parse("Template.ShortHelp", itemTextNode);
            template.LongHelp.Parse("Template.LongHelp", itemTextNode);

            template.References.AddRange(ParseReferences(context, template, itemTextNode, template.BaseTemplates.Value));

            // section
            var templateSection = context.ParseContext.Factory.TemplateSection(TextNode.Empty);
            template.Sections.Add(templateSection);
            templateSection.SectionName.SetValue("Fields");
            templateSection.Icon.SetValue("Applications/16x16/form_blue.png");

            // fields
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

                    var templateField = context.ParseContext.Factory.TemplateField(template);
                    templateSection.Fields.Add(templateField);

                    templateField.FieldName.Parse(child);
                    templateField.Type.Parse("Field.Type", child, "Single-Line Text");
                    templateField.Shared = string.Compare(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
                    templateField.Unversioned = string.Compare(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
                    templateField.Source.Parse("Field.Source", child);
                    templateField.ShortHelp.Parse("Field.ShortHelp", child);
                    templateField.LongHelp.Parse("Field.LongHelp", child);
                    templateField.SortOrder.Parse("Field.SortOrder", child, nextSortOrder);

                    nextSortOrder = templateField.SortOrder.Value + 100;

                    template.References.AddRange(ParseReferences(context, template, child, templateField.Source.Value));
                }
            }

            return context.ParseContext.Project.AddOrMerge(context.ParseContext, template);
        }
    }
}
