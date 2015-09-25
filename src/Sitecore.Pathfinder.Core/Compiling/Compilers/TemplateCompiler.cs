using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    [Export(typeof(ICompiler))]
    public class TemplateCompiler : CompilerBase
    {
        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            return item.SourceTextNodes.Select(n => n.GetAttributeValue("Template.CreateFromFields")).Any(createTemplate => string.Equals(createTemplate, "true", StringComparison.OrdinalIgnoreCase));
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return;
            }

            var templateIdOrPathTextNode = item.SourceTextNodes.Select(n => n.GetAttributeTextNode("Template")).FirstOrDefault(t => t != null);
            if (templateIdOrPathTextNode == null)
            {
                context.Trace.TraceError(Texts.The__Template__attribute_must_be_specified_when__Template_CreateFromFields__equals_true_, TraceHelper.GetTextNode(item));
            }

            var itemTextNode = item.SourceTextNodes.First();
            var itemIdOrPath = item.TemplateIdOrPath;

            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);
            var guid = StringHelper.GetGuid(item.Project, itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath));
            var template = context.Factory.Template(item.Project, guid, itemTextNode, item.DatabaseName, itemName, itemIdOrPath);

            template.ItemName = itemName;
            template.ItemNameProperty.AddSourceTextNode(templateIdOrPathTextNode);
            template.ItemNameProperty.SourcePropertyFlags = SourcePropertyFlags.IsQualified;

            template.IconProperty.Parse("Template.Icon", itemTextNode);
            template.BaseTemplatesProperty.Parse("Template.BaseTemplates", itemTextNode, Constants.Templates.StandardTemplate);
            template.ShortHelpProperty.Parse("Template.ShortHelp", itemTextNode);
            template.LongHelpProperty.Parse("Template.LongHelp", itemTextNode);
            template.IsEmittable = string.Compare(itemTextNode.GetAttributeValue("IsEmittable"), "False", StringComparison.OrdinalIgnoreCase) != 0;
            template.IsExternalReference = string.Compare(itemTextNode.GetAttributeValue("IsExternalReference"), "True", StringComparison.OrdinalIgnoreCase) == 0;

            if (!template.IsExternalReference)
            {
                template.References.AddRange(context.ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));
            }

            if (item.Fields.Any())
            {
                // section
                var templateSection = context.Factory.TemplateSection(TextNode.Empty);
                template.Sections.Add(templateSection);
                templateSection.SectionNameProperty.SetValue("Fields");
                templateSection.IconProperty.SetValue("Applications/16x16/form_blue.png");

                // fields
                var nextSortOrder = 0;
                foreach (var field in item.Fields)
                {
                    var child = field.SourceTextNodes.First();

                    // ignore standard fields
                    if (item.Project.Options.StandardTemplateFields.Contains(field.FieldName, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var templateField = template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => f.FieldName == field.FieldName);
                    if (templateField == null)
                    {
                        templateField = context.Factory.TemplateField(template, child);
                        templateSection.Fields.Add(templateField);

                        templateField.FieldNameProperty.SetValue(field.FieldNameProperty);
                    }
                    else
                    {
                        // todo: multiple sources?
                        templateField.FieldNameProperty.AddSourceTextNode(field.FieldNameProperty.SourceTextNode);
                    }

                    templateField.TypeProperty.TryParse("Field.Type", child, "Single-Line Text");
                    templateField.Shared |= string.Equals(child.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase);
                    templateField.Unversioned |= string.Equals(child.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase);
                    templateField.SourceProperty.TryParse("Field.Source", child);
                    templateField.ShortHelpProperty.TryParse("Field.ShortHelp", child);
                    templateField.LongHelpProperty.TryParse("Field.LongHelp", child);
                    templateField.SortOrderProperty.TryParse("Field.SortOrder", child, nextSortOrder);

                    nextSortOrder = templateField.SortOrder + 100;

                    // todo: added multiple times if merged
                    template.References.AddRange(context.ReferenceParser.ParseReferences(template, templateField.SourceProperty));
                }
            }

            item.Project.AddOrMerge(template);
        }
    }
}