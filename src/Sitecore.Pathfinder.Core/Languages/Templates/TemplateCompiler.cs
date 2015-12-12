using System;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.Templates
{
    public class TemplateCompiler : CompilerBase
    {
        public TemplateCompiler() : base(1000)
        {
        }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            return item.SourceTextNodes.Select(n => n.GetAttributeValue("Template.CreateFromFields")).Any(value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var templateIdOrPathTextNode = item.SourceTextNodes.Select(n => n.GetAttribute("Template")).FirstOrDefault(t => t != null);
            if (templateIdOrPathTextNode == null)
            {
                context.Trace.TraceError(Msg.C1051, Texts.The__Template__attribute_must_be_specified_when__Template_CreateFromFields__equals_true_, TraceHelper.GetTextNode(item));
            }

            var itemTextNode = item.SourceTextNodes.First();
            var itemIdOrPath = item.TemplateIdOrPath;

            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);
            var guid = StringHelper.GetGuid(item.Project, itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath));
            var template = context.Factory.Template(item.Project, guid, itemTextNode, item.DatabaseName, itemName, itemIdOrPath);

            template.ItemNameProperty.AddSourceTextNode(templateIdOrPathTextNode);
            template.ItemNameProperty.Flags = SourcePropertyFlags.IsQualified;

            template.IconProperty.Parse("Template.Icon", itemTextNode);
            template.BaseTemplatesProperty.Parse("Template.BaseTemplates", itemTextNode, Constants.Templates.StandardTemplate);
            template.ShortHelpProperty.Parse("Template.ShortHelp", itemTextNode);
            template.LongHelpProperty.Parse("Template.LongHelp", itemTextNode);
            template.IsEmittable = item.IsEmittable;
            template.IsImport = item.IsImport || string.Equals(itemTextNode.GetAttributeValue(Constants.Fields.IsImport), "True", StringComparison.OrdinalIgnoreCase);

            if (!template.IsImport)
            {
                template.References.AddRange(context.ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));
            }

            if (item.Fields.Any())
            {
                var templateSectionGuid = StringHelper.GetGuid(projectItem.Project, template.ItemIdOrPath + "/Fields");

                // section
                var templateSection = context.Factory.TemplateSection(template, templateSectionGuid, TextNode.Empty);
                template.Sections.Add(templateSection);
                templateSection.SectionNameProperty.SetValue("Fields");
                templateSection.IconProperty.SetValue("Applications/16x16/form_blue.png");

                // fields
                var nextSortOrder = 0;
                foreach (var field in item.Fields)
                {
                    var childNode = field.SourceTextNodes.First();

                    // ignore standard fields
                    if (item.Project.Options.StandardTemplateFields.Contains(field.FieldName, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var templateField = template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => f.FieldName == field.FieldName);
                    if (templateField == null)
                    {
                        var templateFieldGuid = StringHelper.GetGuid(projectItem.Project, template.ItemIdOrPath + "/Fields/" + field.FieldName);
                        templateField = context.Factory.TemplateField(template, templateFieldGuid, childNode);
                        templateSection.Fields.Add(templateField);

                        templateField.FieldNameProperty.SetValue(field.FieldNameProperty);
                        templateField.TypeProperty.Parse("Field.Type", childNode, "Single-Line Text");
                        templateField.SortOrderProperty.Parse("Field.SortOrder", childNode, nextSortOrder);
                    }
                    else
                    {
                        // todo: multiple sources?
                        templateField.FieldNameProperty.AddSourceTextNode(field.FieldNameProperty.SourceTextNode);
                        templateField.TypeProperty.ParseIfHasAttribute("Field.Type", childNode);
                        templateField.SortOrderProperty.ParseIfHasAttribute("Field.SortOrder", childNode);
                    }

                    templateField.Shared |= string.Equals(childNode.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase);
                    templateField.Unversioned |= string.Equals(childNode.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase);
                    templateField.SourceProperty.ParseIfHasAttribute("Field.Source", childNode);
                    templateField.ShortHelpProperty.ParseIfHasAttribute("Field.ShortHelp", childNode);
                    templateField.LongHelpProperty.ParseIfHasAttribute("Field.LongHelp", childNode);

                    nextSortOrder = templateField.SortOrder + 100;

                    // todo: added multiple times if merged
                    template.References.AddRange(context.ReferenceParser.ParseReferences(template, templateField.SourceProperty));
                }
            }

            item.Project.AddOrMerge(template);
        }
    }
}