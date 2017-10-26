// © 2015-2017 by Jakob Christensen. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class FieldCollection : LockableList<Field>
    {
        [NotNull]
        private readonly Item _item;

        public FieldCollection([NotNull] Item item) : base(item)
        {
            _item = item;
        }

        /*
        protected override IEnumerable<Field> FilteredList {
            get
            {
                if (_item.Language == null)
                {
                    return List;
                }

                return this[_item.Language, _item.Version];
            }
        }
        */

        [CanBeNull]
        public Field this[[NotNull] string fieldName, [CanBeNull] Language language = null, [CanBeNull] Version version = null] => GetField(fieldName, language, version);

        [CanBeNull]
        public Field this[Guid fieldId, [CanBeNull] Language language = null, [CanBeNull] Version version = null] => GetField(fieldId, language, version);

        [ItemNotNull, NotNull]
        public IEnumerable<Field> this[[NotNull] Language language, [CanBeNull] Version version = null]
        {
            get
            {
                var fields = this.Where(f => f.TemplateField.Shared || f.Language == language).ToArray();
                foreach (var field in fields)
                {
                    if (field.TemplateField.Shared || field.TemplateField.Unversioned)
                    {
                        yield return field;

                        continue;
                    }

                    if (version != null)
                    {
                        var isLatestVersion = _item.Versions.IsLatestVersion(language, version);
                        if (field.Version == version || isLatestVersion && field.Version == Version.Latest)
                        {
                            yield return field;
                        }

                        continue;
                    }

                    if (field.Version == Version.Latest)
                    {
                        yield return field;

                        continue;
                    }

                    var latest = fields.Where(f => f.FieldName == field.FieldName).Max(f => f.Version.Number);
                    if (field.Version.Number == latest)
                    {
                        yield return field;
                    }
                }
            }
        }

        [CanBeNull]
        public Field GetField([NotNull] string fieldName, [CanBeNull] Language language = null, [CanBeNull] Version version = null)
        {
            if (fieldName.IsGuidOrSoftGuid())
            {
                if (!Guid.TryParse(fieldName, out var guid))
                {
                    guid = StringHelper.ToGuid(fieldName);
                }

                return GetField(guid);
            }

            var fields = this.Where(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase)).ToArray();
            return GetField(fields, language, version);
        }

        [CanBeNull]
        public Field GetField(Guid fieldId, [CanBeNull] Language language = null, [CanBeNull] Version version = null)
        {
            var fields = this.Where(f => f.FieldId == fieldId).ToArray();
            return GetField(fields, language, version);
        }

        [NotNull]
        public string GetFieldValue([NotNull] string fieldName, [CanBeNull] Language language = null, [CanBeNull] Version version = null)
        {
            return GetField(fieldName, language, version)?.Value ?? string.Empty;
        }

        [NotNull]
        public string GetFieldValue(Guid fieldId, [CanBeNull] Language language = null, [CanBeNull] Version version = null)
        {
            return GetField(fieldId, language, version)?.Value ?? string.Empty;
        }

        [CanBeNull]
        protected Field GetField([NotNull, ItemNotNull] Field[] fields, [CanBeNull] Language language, [CanBeNull] Version version)
        {
            if (!fields.Any())
            {
                return null;
            }

            var templateField = fields.First().TemplateField;

            if (templateField.Shared)
            {
                return fields.First();
            }

            // if language is not specified, use Context language
            if (language == null)
            {
                language = _item.Project.Context.Language;
            }

            if (templateField.Unversioned)
            {
                var versions = fields.Where(f => f.Language == language).ToArray();

                var latestVersion = versions.FirstOrDefault(f => f.Version == Version.Latest);
                if (latestVersion != null)
                {
                    return latestVersion;
                }

                var latestVersionNumber = versions.Max(v => v.Version.Number);
                return versions.First(f => f.Version.Number == latestVersionNumber);
            }

            return fields.FirstOrDefault(f => f.Language == language && f.Version == version);
        }
    }
}
