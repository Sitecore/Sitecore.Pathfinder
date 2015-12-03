// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class XPathItem : IXPathItem
    {
        [CanBeNull]
        private string _parentPath;

        public XPathItem([NotNull] IProject project, [NotNull] string databaseName, [NotNull] string itemPath)
        {
            Project = project;
            DatabaseName = databaseName;
            ItemPath = itemPath;

            var n = ItemPath.LastIndexOf('/');
            ItemName = ItemPath.Mid(n + 1);
        }

        [NotNull]
        public string DatabaseName { get; }

        public string this[string name] => string.Empty;

        public string ItemId => string.Empty;

        public string ItemName { get; }

        [NotNull]
        public string ParentItemPath => _parentPath ?? (_parentPath = PathHelper.GetItemParentPath(ItemPath));

        public string TemplateId => string.Empty;

        public string TemplateName => string.Empty;

        public string ItemPath { get; }

        [NotNull]
        protected IProject Project { get; }

        public IEnumerable<IXPathItem> GetChildren()
        {
            var childNames = new HashSet<string>();

            foreach (var child in Project.GetItems(DatabaseName).Where(i => string.Equals(i.ParentItemPath, ItemPath, StringComparison.OrdinalIgnoreCase)))
            {
                yield return child;
                childNames.Add(child.ItemName);
            }

            // yield virtual paths that are used by items deeper in the hierachy - tricky, tricky
            var itemIdOrPath = ItemPath + "/";
            foreach (var descendent in Project.GetItems(DatabaseName).Where(i => i.ItemIdOrPath.StartsWith(itemIdOrPath, StringComparison.OrdinalIgnoreCase)))
            {
                var n = descendent.ItemIdOrPath.IndexOf('/', itemIdOrPath.Length);
                if (n < 0)
                {
                    continue;
                }

                var childName = descendent.ItemIdOrPath.Mid(itemIdOrPath.Length, n - itemIdOrPath.Length);
                if (childNames.Contains(childName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return new XPathItem(Project, DatabaseName, itemIdOrPath + childName);
                childNames.Add(childName);
            }
        }

        public IXPathItem GetParent()
        {
            if (string.IsNullOrEmpty(ParentItemPath))
            {
                return null;
            }

            var item = Project.FindQualifiedItem<Item>(DatabaseName, ParentItemPath) as IXPathItem;
            return item ?? new XPathItem(Project, DatabaseName, ParentItemPath);
        }
    }
}
