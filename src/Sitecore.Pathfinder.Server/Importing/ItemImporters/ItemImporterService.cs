// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Builders;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
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

        public virtual Item ImportItem(IProject project, Data.Items.Item item, ILanguage language, [ItemNotNull] IEnumerable<string> excludedFields)
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
