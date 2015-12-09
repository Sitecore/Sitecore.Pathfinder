// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Importing.ItemImporters
{
    [Export(typeof(IItemImporterService))]
    public class ItemImporterService : IItemImporterService
    {
        [ImportingConstructor]
        public ItemImporterService([Diagnostics.NotNull] IFactoryService factory, [ImportMany, Diagnostics.NotNull, ItemNotNull] IEnumerable<IFieldValueImporter> fieldValueImporters)
        {
            Factory = factory;
            FieldValueImporters = fieldValueImporters;
        }

        [Diagnostics.NotNull]
        protected IFactoryService Factory { get; }

        [Diagnostics.NotNull, ItemNotNull]
        protected IEnumerable<IFieldValueImporter> FieldValueImporters { get; }

        public virtual Projects.Items.Item ImportItem(IProject project, Data.Items.Item item, ILanguage language, [ItemNotNull] IEnumerable<string> excludedFields)
        {
            var itemBuilder = new ItemBuilder(Factory)
            {
                DatabaseName = item.Database.Name,
                Guid = item.ID.ToString(),
                ItemName = item.Name,
                TemplateIdOrPath = item.Template.InnerItem.Paths.Path,
                ItemIdOrPath = item.Paths.Path
            };

            var versions = item.Versions.GetVersions(true);
            var sharedFields = item.Fields.Where(f => f.Shared && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value)).ToList();
            var unversionedFields = versions.SelectMany(i => i.Fields.Where(f => !f.Shared && f.Unversioned && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value))).ToList();
            var versionedFields = versions.SelectMany(i => i.Fields.Where(f => !f.Shared && !f.Unversioned && !excludedFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase) && !f.ContainsStandardValue && !string.IsNullOrEmpty(f.Value))).ToList();

            // shared fields
            foreach (var field in sharedFields.OrderBy(f => f.Name))
            {
                var value = ImportFieldValue(field, item, language);
                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            // unversioned fields
            foreach (var field in unversionedFields.OrderBy(f => f.Name))
            {
                var value = ImportFieldValue(field, item, language);
                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value,
                    Language = field.Language.Name
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            // versioned fields
            foreach (var field in versionedFields.OrderBy(f => f.Name))
            {
                var value = ImportFieldValue(field, item, language);
                var fieldBuilder = new FieldBuilder(Factory)
                {
                    FieldName = field.Name,
                    Value = value,
                    Language = field.Language.Name,
                    Version = field.Item.Version.Number
                };

                itemBuilder.Fields.Add(fieldBuilder);
            }

            return itemBuilder.Build(project, TextNode.Empty);
        }

        public virtual Template ImportTemplate(IProject project, Data.Items.Item item)
        {
            var templateItem = new TemplateItem(item);

            var templateBuilder = new TemplateBuilder(Factory);
            templateBuilder.DatabaseName = templateItem.Database.Name;
            templateBuilder.Guid = templateItem.ID.ToString();
            templateBuilder.TemplateName = templateItem.Name;
            templateBuilder.ItemIdOrPath = templateItem.InnerItem.Paths.Path;
            templateBuilder.Icon = templateItem.InnerItem.Appearance.Icon;
            templateBuilder.ShortHelp = templateItem.InnerItem.Help.ToolTip;
            templateBuilder.LongHelp = templateItem.InnerItem.Help.Text;

            var baseTemplates = templateItem.BaseTemplates;
            if (baseTemplates.Length > 1 || (baseTemplates.Length == 1 && baseTemplates[0].ID != TemplateIDs.StandardTemplate))
            {
                templateBuilder.BaseTemplates = string.Join("|", baseTemplates.Select(i => i.InnerItem.Paths.Path));
            }

            foreach (var templateSectionItem in templateItem.GetSections())
            {
                var templateSectionBuilder = new TemplateSectionBuilder(Factory).With(templateBuilder, TextNode.Empty);
                templateSectionBuilder.SectionId = templateSectionItem.ID.ToString();
                templateSectionBuilder.SectionName = templateSectionItem.Name;

                foreach (var templateFieldItem in templateSectionItem.GetFields())
                {
                    var templateFieldBuilder = new TemplateFieldBuilder(Factory).With(templateSectionBuilder, TextNode.Empty);
                    templateFieldBuilder.FieldId = templateFieldItem.ID.ToString();
                    templateFieldBuilder.FieldName = templateFieldItem.Name;
                    templateFieldBuilder.Source = templateFieldItem.Source;
                    templateFieldBuilder.Type = templateFieldItem.Type;
                    templateFieldBuilder.TemplateFieldShortHelp = templateFieldItem.InnerItem.Help.ToolTip;
                    templateFieldBuilder.TemplateFieldLongHelp = templateFieldItem.InnerItem.Help.Text;

                    templateSectionBuilder.Fields.Add(templateFieldBuilder);
                }

                templateBuilder.Sections.Add(templateSectionBuilder);
            }

            return templateBuilder.Build(project, TextNode.Empty);
        }

        [Diagnostics.NotNull]
        protected virtual string ImportFieldValue([Diagnostics.NotNull] Data.Fields.Field field, [Diagnostics.NotNull] Data.Items.Item item, [Diagnostics.NotNull] ILanguage language)
        {
            var value = field.Value;

            foreach (var fieldValueImporter in FieldValueImporters)
            {
                if (fieldValueImporter.CanImport(field, item, language, value))
                {
                    value = fieldValueImporter.Import(field, item, language, value);
                }
            }

            return value;
        }
    }
}
