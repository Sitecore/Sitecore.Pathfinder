using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Querying;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class Templates : Check
    {
        [ImportingConstructor]
        public Templates([NotNull] IQueryService queryService)
        {
            QueryService = queryService;
        }

        [NotNull]
        protected IQueryService QueryService { get; }

        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidSpacesInTemplateNames(ICheckerContext context)
        {
            return 
                from template in context.Project.Templates
                where
                    template.ItemName.IndexOf(' ') >= 0
                select
                    Warning(Msg.C1012, "Avoid spaces in template names. Use a display name instead", template.ItemName, template.ItemNameProperty);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidEmptyTemplate(ICheckerContext context)
        {
            return 
                from template in context.Project.Templates
                where
                    !template.Sections.Any() && 
                    template.BaseTemplates == Constants.Templates.StandardTemplate
                select
                    Warning(Msg.C1013, "Empty templates should be avoided. Consider using the 'Folder' template instead", template.ItemName, template);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> TemplateShouldHaveIcon(ICheckerContext context)
        {
            return 
                from template in context.Project.Templates
                where
                    string.IsNullOrEmpty(template.Icon)
                select
                    Warning(Msg.C1020, "Template should should have an icon", template.ItemName, template);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidEmptyTemplateSection(ICheckerContext context)
        {
            return 
                from template in context.Project.Templates
                from section in template.Sections
                where
                    !section.Fields.Any()
                select
                    Warning(Msg.C1023, "Avoid empty template section", section.SectionName, section);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> DeleteUnusedTemplates(ICheckerContext context)
        {
            return
                from template in context.Project.Templates
                let references = QueryService.FindUsages(context.Project, template.QualifiedName)
                where
                    !references.Any()
                select
                    Warning(Msg.C1025, "Template is not referenced and can be deleted", template.ItemName, template);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidDuplicateFieldNames(ICheckerContext context)
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
                            yield return Warning(Msg.C1023, "Avoid duplicate template field names", $"The template contains two or more field with the same name \"{field0.FieldName}\". Even if these fields are located in different sections, it is still not recommended as the name is ambiguous. Rename one or more of the fields", field0.FieldNameProperty, field1.FieldNameProperty, field0, field1);
                        }
                    }
                }
            }
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> AvoidDeprecatedFieldType(ICheckerContext context)
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
                        yield return Warning(Msg.C1022, "Avoid deprecated field type", $"The field type \"{type}\" is deprecated. Use the \"{newType}\" field type instead", field.TypeProperty, field);
                    }
                }
            }
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> UseIdInsteadOfPath(ICheckerContext context)
        {
            return
                from template in context.Project.Templates
                from field in template.Fields
                where
                    field.Source.IndexOf('/') >= 0
                select
                    Warning(Msg.C1026, "Use IDs instead of paths in template fields", $"The template field Source field contains the path \"{field.Source}\". It is recommended to use IDs instead.", field.SourceProperty, field);
        }

        [Export("Check")]
        public IEnumerable<Diagnostic> FieldIdTemplateFieldId(ICheckerContext context)
        {
            return 
                from item in context.Project.Items
                from field in item.Fields
                let 
                    templateField = field.TemplateField
                where 
                    templateField != TemplateField.Empty && 
                    templateField.Uri.Guid != field.FieldId
                select 
                    Warning(Msg.C1024, "Field ID and Template Field ID differs", $"FieldId: {field.FieldId.Format()}, TemplateFieldId: {templateField.Uri.Guid.Format()}", field.FieldIdProperty, field);
        }
    }
}