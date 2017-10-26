// © 2015-2017 by Jakob Christensen. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

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
        public IEnumerable<Field> GetSharedFields()
        {
            return _item.Fields.Where(f => f.TemplateField.Shared);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Field> GetUnversionedFields([NotNull] Language language)
        {
            return _item.Fields.Where(f => !f.TemplateField.Shared && f.TemplateField.Unversioned && f.Language == language);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Field> GetVersionedFields([NotNull] Language language, [NotNull] Version version)
        {
            return _item.Fields.Where(f => !f.TemplateField.Shared && !f.TemplateField.Unversioned && f.Language == language && f.Version == version);
        }

        [NotNull]
        public VersionedItem GetVersionedItem([NotNull] Language language, [NotNull] Version version)
        {
            var versionedItem = new VersionedItem(_item.Database, _item.Uri.Guid, _item.ItemName, _item.ItemIdOrPath, _item.TemplateIdOrPath, language, version);

            // todo: consider if fields should be cloned
            versionedItem.Fields.AddRange(_item.Fields[language, version]);

            return versionedItem;
        }

        [ItemNotNull, NotNull]
        public IEnumerable<Version> GetVersions([NotNull] Language language)
        {
            var versions = _item.Fields.Where(f => f.Language == language).ToArray();

            return versions.Select(f => f.Version.Number).Where(n => n != Version.Latest.Number).Distinct().OrderBy(n => n).Select(n => new Version(n));
        }

        public bool IsLatestVersion([NotNull] Language language, [NotNull] Version version)
        {
            return version == GetLatestVersion(language);
        }
    }
}
