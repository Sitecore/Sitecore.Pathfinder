// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class ChildrenCollection : IEnumerable<Item>
    {
        [NotNull]
        private readonly Item _item;

        public ChildrenCollection([NotNull] Item item)
        {
            _item = item;
        }

        [CanBeNull]
        public Item this[[NotNull] string childName]
        {
            get { return GetChildren().FirstOrDefault(i => string.Equals(i.ItemName, childName, StringComparison.OrdinalIgnoreCase)); }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return GetChildren().GetEnumerator();
        }

        [ItemNotNull, NotNull]
        protected virtual IEnumerable<Item> GetChildren()
        {
            return _item.Project.Indexes.ChildrenIndex.Where<Item>(_item.Database, _item.ItemIdOrPath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
