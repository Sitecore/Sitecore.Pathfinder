// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

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

        [CanBeNull]
        public Field this[[NotNull] string fieldName, [CanBeNull] Language language = null, [CanBeNull] Version version = null] => GetField(fieldName, language, version);

        [CanBeNull]
        public Field this[Guid fieldId, [CanBeNull] Language language = null, [CanBeNull] Version version = null] => GetField(fieldId, language, version);

        [CanBeNull]
        public Field GetField([NotNull] string fieldName, [CanBeNull] Language language = null, [CanBeNull] Version version = null)
        {
            if (fieldName.IsGuidOrSoftGuid())
            {
                if (!Guid.TryParse(fieldName, out Guid guid))
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

        [ItemNotNull, NotNull]
        public IEnumerable<Field> GetFieldsInVersion([NotNull] Language language, [NotNull] Version version)
        {
            return this.Where(f => f.TemplateField.Shared || f.TemplateField.Unversioned && f.Language == language || !f.TemplateField.Shared && !f.TemplateField.Unversioned && f.Language == language && f.Version == version);
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
                var latestVersion = versions.Max(v => v.Version.Number);
                return versions.First(f => f.Version.Number == latestVersion);
            }

            return fields.FirstOrDefault(f => f.Language == language && f.Version == version);
        }
    }
}
