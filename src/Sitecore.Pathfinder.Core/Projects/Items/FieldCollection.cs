// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class FieldCollection : List<Field>
    {
        [NotNull]
        private readonly Item _item;

        public FieldCollection([NotNull] Item item)
        {
            _item = item;
        }

        [CanBeNull]
        public Field this[[NotNull] string fieldName] => this.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));

        [CanBeNull]
        public Field this[Guid fieldId] => this.FirstOrDefault(f => f.FieldId == fieldId);

        [NotNull]
        public string GetFieldValue([NotNull] string fieldName, [NotNull] string language, int version)
        {
            return this.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase) && string.Equals(f.Language, language, StringComparison.OrdinalIgnoreCase) && f.Version == version)?.Value ?? string.Empty;
        }

        [NotNull]
        public string GetFieldValue(Guid fieldId, [NotNull] string language, int version)
        {
            return this.FirstOrDefault(f => f.FieldId == fieldId && string.Equals(f.Language, language, StringComparison.OrdinalIgnoreCase) && f.Version == version)?.Value ?? string.Empty;
        }
    }
}
