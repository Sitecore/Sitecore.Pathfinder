// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing.References;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.Templates
{
    [Export(typeof(ICompiler)), Shared]
    public class TemplateCompiler : CompilerBase
    {
        [ImportingConstructor]
        public TemplateCompiler([NotNull] ITraceService trace, [NotNull] IReferenceParserService referenceParser) : base(1000)
        {
            Trace = trace;
            ReferenceParser = referenceParser;
        }

        [NotNull]
        protected IReferenceParserService ReferenceParser { get; }

        [NotNull]
        protected ITraceService Trace { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            return item.GetTextNodes().Select(n => n.GetAttributeValue("Template.CreateFromFields")).Any(value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var standardTemplate = item.Database.FindQualifiedItem<Template>(Constants.Templates.StandardTemplateId);
            if (standardTemplate == null)
            {
                Trace.TraceWarning(Msg.C1135, "Standard Template not found - are you missing a reference?");
                return;
            }

            var standardFields = standardTemplate.GetAllFields().ToArray();

            var templateIdOrPathTextNode = item.GetTextNodes().Select(n => n.GetAttribute("Template")).FirstOrDefault(t => t != null);
            if (templateIdOrPathTextNode == null)
            {
                Trace.TraceError(Msg.C1051, Texts.The__Template__attribute_must_be_specified_when__Template_CreateFromFields__equals_true_, TraceHelper.GetTextNode(item));
            }

            var itemTextNode = item.SourceTextNode;
            var itemIdOrPath = item.TemplateIdOrPath;

            var itemName = itemIdOrPath.Mid(itemIdOrPath.LastIndexOf('/') + 1);
            var guid = StringHelper.GetGuid(item.Project, itemTextNode.GetAttributeValue("Template.Id", itemIdOrPath));
            var template = context.Factory.Template(item.Database, guid, itemName, itemIdOrPath).With(itemTextNode);

            template.ItemNameProperty.AddSourceTextNode(templateIdOrPathTextNode);
            template.ItemNameProperty.Flags = SourcePropertyFlags.IsQualified;

            template.IconProperty.Parse("Template.Icon", itemTextNode);
            template.BaseTemplatesProperty.Parse("Template.BaseTemplates", itemTextNode, Constants.Templates.StandardTemplateId);
            template.ShortHelpProperty.Parse("Template.ShortHelp", itemTextNode);
            template.LongHelpProperty.Parse("Template.LongHelp", itemTextNode);
            template.IsEmittable = item.IsEmittable;
            template.IsImport = item.IsImport || string.Equals(itemTextNode.GetAttributeValue(Constants.Fields.IsImport), "True", StringComparison.OrdinalIgnoreCase);

            if (!template.IsImport)
            {
                template.References.AddRange(ReferenceParser.ParseReferences(template, template.BaseTemplatesProperty));
            }

            if (item.Fields.Any())
            {
                var templateSectionGuid = StringHelper.GetGuid(projectItem.Project, template.ItemIdOrPath + "/Fields");

                // section
                var templateSection = context.Factory.TemplateSection(template, templateSectionGuid);
                template.Sections.Add(templateSection);
                templateSection.SectionNameProperty.SetValue("Fields");
                templateSection.IconProperty.SetValue("Applications/16x16/form_blue.png");

                // fields
                var nextSortOrder = 0;
                foreach (var field in item.Fields)
                {
                    var childNode = field.SourceTextNode;

                    // ignore standard fields
                    if (standardFields.Any(f => string.Equals(f.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var templateField = template.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => f.FieldName == field.FieldName);
                    if (templateField == null)
                    {
                        var templateFieldGuid = StringHelper.GetGuid(projectItem.Project, template.ItemIdOrPath + "/Fields/" + field.FieldName);
                        templateField = context.Factory.TemplateField(template, templateFieldGuid).With(childNode);
                        templateSection.Fields.Add(templateField);

                        templateField.FieldNameProperty.SetValue(field.FieldNameProperty);
                        templateField.TypeProperty.Parse("Field.Type", childNode, "Single-Line Text");
                        templateField.SortorderProperty.Parse("Field.SortOrder", childNode, nextSortOrder);
                    }
                    else
                    {
                        // todo: multiple sources?
                        templateField.FieldNameProperty.AddSourceTextNode(field.FieldNameProperty.SourceTextNode);
                        templateField.TypeProperty.ParseIfHasAttribute("Field.Type", childNode);
                        templateField.SortorderProperty.ParseIfHasAttribute("Field.SortOrder", childNode);
                    }

                    templateField.Shared |= string.Equals(childNode.GetAttributeValue("Field.Sharing"), "Shared", StringComparison.OrdinalIgnoreCase);
                    templateField.Unversioned |= string.Equals(childNode.GetAttributeValue("Field.Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase);
                    templateField.SourceProperty.ParseIfHasAttribute("Field.Source", childNode);
                    templateField.ShortHelpProperty.ParseIfHasAttribute("Field.ShortHelp", childNode);
                    templateField.LongHelpProperty.ParseIfHasAttribute("Field.LongHelp", childNode);

                    nextSortOrder = templateField.Sortorder + 100;

                    // todo: added multiple times if merged
                    template.References.AddRange(ReferenceParser.ParseReferences(template, templateField.SourceProperty));
                }
            }

            context.Project.AddOrMerge(template);
        }
    }
}
