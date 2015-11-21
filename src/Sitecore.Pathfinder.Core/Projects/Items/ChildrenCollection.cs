// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects
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
            get { return _item.GetChildren().FirstOrDefault(i => string.Equals(i.ItemName, childName, StringComparison.OrdinalIgnoreCase)); }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _item.GetChildren().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
