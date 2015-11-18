// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.T4.Code
{
    public class CodeItemChildCollection : IEnumerable<CodeItem>
    {
        [NotNull]
        private readonly CodeItem _item;

        public CodeItemChildCollection([NotNull] CodeItem item)
        {
            _item = item;
        }

        [CanBeNull]
        public CodeItem this[[NotNull] string childName]
        {
            get
            {
                var child = _item.InnerItem.GetChildren().FirstOrDefault(i => string.Equals(i.ItemName, childName, StringComparison.OrdinalIgnoreCase));
                return child == null ? null : new CodeItem(_item.Project, child);
            }
        }

        public IEnumerator<CodeItem> GetEnumerator()
        {
            return _item.InnerItem.GetChildren().Select(i => new CodeItem(_item.Project, i)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
