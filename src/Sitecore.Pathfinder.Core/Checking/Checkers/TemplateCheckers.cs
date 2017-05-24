// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    [Export(typeof(IChecker)), Shared]
    public class TemplateCheckers : Checker
    {
        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> AvoidDeprecatedFieldType([NotNull] ICheckerContext context)
        {
            foreach (var template in context.Project.Templates)
            {
                foreach (var field in template.Fields)
                {
                    var type = field.Type;
                    string newType = null;

                    switch (type.ToLowerInvariant())
                    {
                        case "text":
                            newType = "Single-Line Text";
                            break;
                        case "html":
                            newType = "Rich Text";
                            break;
                        case "link":
                            newType = "General Link";
                            break;
                        case "lookup":
                            newType = "Droplink";
                            break;
                        case "memo":
                            newType = "Multi-Line Text";
                            break;
                        case "reference":
                            newType = "Droptree";
                            break;
                        case "server file":
                            newType = "Single-Line Text";
                            break;
                        case "tree":
                            newType = "Droptree";
                            break;
                        case "treelist":
                            newType = "TreelistEx";
                            break;
                        case "valuelookup":
                            newType = "Droplist";
                            break;
                    }

                    if (!string.IsNullOrEmpty(newType))
                    {
                        yield return Warning(Msg.C1022, "Avoid deprecated field type", TraceHelper.GetTextNode(field.TypeProperty, field, template), $"The field type \"{type}\" is deprecated. Use the \"{newType}\" field type instead");
                    }
                }
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> AvoidDuplicateFieldNames([NotNull] ICheckerContext context)
        {
            foreach (var template in context.Project.Templates)
            {
                var fields = template.GetAllFields().ToArray();

                for (var i0 = 0; i0 < fields.Length - 2; i0++)
                {
                    var field0 = fields[i0];
                    for (var i1 = i0 + 1; i1 < fields.Length - 1; i1++)
                    {
                        var field1 = fields[i1];

                        if (string.Equals(field0.FieldName, field1.FieldName, StringComparison.OrdinalIgnoreCase))
                        {
                            yield return Warning(Msg.C1023, "Avoid duplicate template field names", TraceHelper.GetTextNode(field0.FieldNameProperty, field1.FieldNameProperty, field0, field1, template), $"The template contains two or more fields with the same name \"{field0.FieldName}\". Even if these fields are located in different sections, it is still not recommended as the name is ambiguous. Rename one or more of the fields");
                        }
                    }
                }
            }
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> AvoidEmptyTemplate([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !template.Sections.Any() && template.BaseTemplates == Constants.Templates.StandardTemplateId
                select Warning(Msg.C1013, "Empty templates should be avoided. Consider using the 'Folder' template instead", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> AvoidEmptyTemplateSection([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from section in template.Sections
                where !section.Fields.Any()
                select Warning(Msg.C1118, "Avoid empty template section", TraceHelper.GetTextNode(section, template), section.SectionName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> AvoidSpacesInTemplateNames([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where template.ItemName.IndexOf(' ') >= 0
                select Warning(Msg.C1012, "Avoid spaces in template names. Use a display name instead", TraceHelper.GetTextNode(template.ItemNameProperty, template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> DeleteUnusedTemplates([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !context.Project.Items.Any(i => i.References.Any(r => r.Resolve() == template)) && !context.Project.Templates.Any(i => i.References.Any(r => r.Resolve() == template))
                select Warning(Msg.C1025, "Template is not referenced and can be deleted", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> FieldIdTemplateFieldId([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                from field in item.Fields
                let templateField = field.TemplateField
                where templateField != TemplateField.Empty && templateField.Uri.Guid != field.FieldId
                select Warning(Msg.C1024, "Field ID and Template Field ID differ", TraceHelper.GetTextNode(field.FieldIdProperty, field, item), $"FieldId: {field.FieldId.Format()}, TemplateFieldId: {templateField.Uri.Guid.Format()}");
        }

        [ItemNotNull, NotNull, Check]
        public IEnumerable<Diagnostic> TemplateShouldHaveIcon([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where string.IsNullOrEmpty(template.Icon)
                select Warning(Msg.C1020, "Template should should have an icon", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> AvoidSettingSharedAndUnversionedInItems([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.TemplateName.Equals("Template field") && item["Shared"].Equals("True") && item["Unversioned"].Equals("True")
                select Warning(Msg.C1119, "In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)", TraceHelper.GetTextNode(item), item.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> AvoidSettingSharedAndUnversionedInTemplates([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                from templateSection in template.Sections
                from templateField in templateSection.Fields
                where templateField.Shared && templateField.Unversioned
                select Warning(Msg.C1120, "In a template field, the 'Shared' field overrides the 'Unversioned' field. To fix, clear the 'Unversioned' field (the field remains shared)", TraceHelper.GetTextNode(templateField, template), templateField.FieldName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> DefaultValueFieldIsObsolete([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.TemplateName.Equals("Template field")
                let defaultValueField = item.Fields["Default Value"]
                where defaultValueField != null && defaultValueField.Value != string.Empty
                select Warning(Msg.C1121, "In a template field, the 'Default value' field is no longer used. To fix, clear the 'Default value' field and set the value on the Standard Values item", TraceHelper.GetTextNode(defaultValueField, item), item.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> TemplateIdOfStandardValuesShouldMatchParentId([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                let parent = item.GetParent()
                where parent != null && item.Paths.IsStandardValuesHolder && item.Template.Uri.Guid != parent.Uri.Guid
                select Error(Msg.C1122, "The Template ID of a Standard Values item should be match the ID of the parent item. To fix, moved the Standard Values item under the correct template", TraceHelper.GetTextNode(item), item.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> TemplateMustLocatedInTemplatesSection([NotNull] ICheckerContext context)
        {
            return from template in context.Project.Templates
                where !template.ItemIdOrPath.StartsWith("/sitecore/templates/")
                select Warning(Msg.C1123, "All templates should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", TraceHelper.GetTextNode(template), template.ItemName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> TemplateNodeOrFolderShouldBeTemplateFolder([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.ItemIdOrPath.StartsWith("/sitecore/templates/") && (item.TemplateName.Equals("Folder") || item.TemplateName.Equals("Node"))
                select Warning(Msg.C1124, "In the '/sitecore/templates' section, folder items use the 'Template folder' template - not the 'Folder' or 'Node' template. To fix, change the template of the item to 'Template Folder", TraceHelper.GetTextNode(item), item.ItemIdOrPath);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> TemplateSectionShouldOnlyContainTemplates([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where item.ItemIdOrPath.StartsWith("/sitecore/templates/") && item.ItemIdOrPath.IndexOf("Branches", StringComparison.Ordinal) < 0 && !item.Paths.IsStandardValuesHolder && !item.TemplateName.Equals("Template") && !item.TemplateName.Equals("Template section") && !item.TemplateName.Equals("Template field") && !item.TemplateName.Equals("Template Folder") && !item.TemplateName.Equals("Node") && !item.TemplateName.Equals("Folder") && !item.TemplateName.Equals("Command Template")
                select Warning(Msg.C1125, "The '/sitecore/templates' section should only contain item with template 'Template', 'Template section', 'Template field', 'Template folder' or standard values items. To fix, move the item outside the '/sitecore/templates' section", TraceHelper.GetTextNode(item), item.TemplateName);
        }

        [ItemNotNull, NotNull, Check]
        protected IEnumerable<Diagnostic> TemplatesMustLocatedInTemplatesSection([NotNull] ICheckerContext context)
        {
            return from item in context.Project.Items
                where !item.ItemIdOrPath.StartsWith("/sitecore/templates/") && (item.TemplateName.Equals("Template") || item.TemplateName.Equals("Template section") || item.TemplateName.Equals("Template Field") || item.TemplateName.Equals("Template Folder"))
                select Warning(Msg.C1126, "All items with template 'Template', 'Template section', 'Template field' and 'Template folder' should be located in the '/sitecore/templates' section. To fix, move the template into the '/sitecore/templates' section", TraceHelper.GetTextNode(item), item.TemplateName);
        }

        /*
        [Check]
        public IEnumerable<Diagnostic> UseIdInsteadOfPath(ICheckerContext context)
        {
            return from template in context.Project.Templates
                from field in template.Fields
                where field.Source.IndexOf('/') >= 0
                select Warning(Msg.C1026, "Use IDs instead of paths in template fields", TraceHelper.GetTextNode(field.SourceProperty, field), $"The template field Source field contains the path \"{field.Source}\". It is recommended to use IDs instead.");
        }
        */
    }
}
