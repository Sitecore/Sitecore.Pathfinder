// © 2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class FieldCollection : LockableList<Field>
    {
        public FieldCollection([NotNull] Item item) : base(item)
        {
        }

        [CanBeNull]
        public Field this[[NotNull] string fieldName] => GetField(fieldName);

        [CanBeNull]
        public Field this[Guid fieldId] => GetField(fieldId);

        [CanBeNull]
        public Field GetField([NotNull] string fieldName)
        {
            // todo: handle languages and versions
            if (fieldName.StartsWith("{") && fieldName.StartsWith("}"))
            {
                Guid guid;
                if (Guid.TryParse(fieldName, out guid))
                {
                    return GetField(guid);
                }
            }

            return this.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
        }

        [CanBeNull]
        public Field GetField(Guid fieldId)
        {
            // todo: handle languages and versions
            return this.FirstOrDefault(f => f.FieldId == fieldId);
        }

        [NotNull]
        public string GetFieldValue([NotNull] string fieldName)
        {
            return GetField(fieldName)?.Value ?? string.Empty;
        }

        [NotNull]
        public string GetFieldValue(Guid fieldId)
        {
            return GetField(fieldId)?.Value ?? string.Empty;
        }

        [NotNull]
        public string GetFieldValue([NotNull] string fieldName, [NotNull] Language language, [NotNull] Version version)
        {
            return this.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase) && f.Language == language && f.Version == version)?.Value ?? string.Empty;
        }

        [NotNull]
        public string GetFieldValue(Guid fieldId, [NotNull] Language language, [NotNull] Version version)
        {
            return this.FirstOrDefault(f => f.FieldId == fieldId && f.Language == language && f.Version == version)?.Value ?? string.Empty;
        }
    }
}
