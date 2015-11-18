// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeItemFieldCollection : IEnumerable<CodeField>
    {
        [NotNull]
        private readonly CodeItem _item;

        public CodeItemFieldCollection([NotNull] CodeItem item)
        {
            _item = item;
        }

        [CanBeNull]
        public CodeField this[[NotNull] string fieldName]
        {
            get
            {
                var field = _item.InnerItem.Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));
                return field == null ? null : new CodeField(_item.Project, _item, field);
            }
        }

        public IEnumerator<CodeField> GetEnumerator()
        {
            return _item.InnerItem.Fields.Select(f => new CodeField(_item.Project, _item, f)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
