// © 2015-2017 by Jakob Christensen. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ItemVersions
    {
        [NotNull]
        private readonly Item _item;

        public ItemVersions([NotNull] Item item)
        {
            _item = item;
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Field> this[[NotNull] Language language, [CanBeNull] Version version = null] => _item.Fields[language, version];

        public int Count([NotNull] Language language)
        {
            var versions = _item.Fields.Where(f => f.Language == language).ToArray();

            return versions.Select(f => f.Version.Number).Distinct().Count();
        }

        [NotNull, ItemNotNull]
        public IEnumerable<Language> GetLanguages() => _item.Fields.Where(f => f.Language != Language.Undefined && f.Language != Language.Empty).Select(f => f.Language).Distinct();

        [NotNull]
        public Version GetLatestVersion([NotNull] Language language)
        {
            var versions = _item.Fields.Where(f => f.Language == language).ToArray();
            if (!versions.Any())
            {
                return Version.Undefined;
            }

            return new Version(versions.Max(f => f.Version.Number));
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Field> GetSharedFields() => _item.Fields.Where(f => f.TemplateField.Shared);

        [ItemNotNull, NotNull]
        public IEnumerable<Field> GetUnversionedFields([NotNull] Language language) => _item.Fields.Where(f => !f.TemplateField.Shared && f.TemplateField.Unversioned && f.Language == language);

        [ItemNotNull, NotNull]
        public IEnumerable<Field> GetVersionedFields([NotNull] Language language, [NotNull] Version version) => _item.Fields.Where(f => !f.TemplateField.Shared && !f.TemplateField.Unversioned && f.Language == language && f.Version == version);

        [NotNull]
        public VersionedItem GetVersionedItem([NotNull] Language language, [NotNull] Version version)
        {
            var versionedItem = (VersionedItem)new VersionedItem(_item.Database, _item.Uri.Guid, _item.ItemName, _item.ItemIdOrPath, _item.TemplateIdOrPath, language, version).With(_item.SourceTextNode);
            versionedItem.ItemNameProperty.SetValue(_item.ItemNameProperty);
            versionedItem.IconProperty.SetValue(_item.IconProperty);
            versionedItem.SortorderProperty.SetValue(_item.SortorderProperty);
            versionedItem.TemplateIdOrPathProperty.SetValue(_item.TemplateIdOrPathProperty);

            versionedItem.IsEmittable = false;
            versionedItem.IsImport = _item.IsImport;

            var fields = _item.Fields[language, version];
            foreach (var f in fields)
            {
                var field = new Field(versionedItem).With(f.SourceTextNode);
                field.FieldIdProperty.SetValue(f.FieldIdProperty);
                field.FieldNameProperty.SetValue(f.FieldNameProperty);
                field.LanguageProperty.SetValue(f.LanguageProperty);
                field.VersionProperty.SetValue(f.VersionProperty);
                field.ValueProperty.SetValue(f.ValueProperty);

                versionedItem.Fields.Add(field);
            }

            return versionedItem;
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Version> GetVersions([NotNull] Language language)
        {
            var versions = _item.Fields.Where(f => f.Language == language).ToArray();
            return versions.Select(f => f.Version.Number).Where(n => n != Version.Latest.Number).Distinct().OrderBy(n => n).Select(n => new Version(n));
        }

        public bool IsLatestVersion([NotNull] Language language, [NotNull] Version version) => version == GetLatestVersion(language);
    }
}
