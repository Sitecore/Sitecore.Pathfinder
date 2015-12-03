// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class XPathItem : IXPathItem
    {
        public XPathItem([NotNull] IProject project, [NotNull] string itemPath)
        {
            Project = project;
            ItemPath = itemPath;

            var n = ItemPath.LastIndexOf('/');
            ItemName = ItemPath.Mid(n + 1);
        }

        public string this[string name] => string.Empty;

        public string ItemId => string.Empty;

        public string ItemName { get; }

        [NotNull]
        protected string ItemPath { get; }

        [NotNull]
        protected IProject Project { get; }

        public string TemplateId => string.Empty;

        public string TemplateName => string.Empty;

        public IEnumerable<IXPathItem> GetChildren()
        {
            var itemIdOrPath = ItemPath + "/";
            var index = itemIdOrPath.Length;

            // todo: return XPath items
            // todo: create items from templates
            // todo: rename ItemBase => DatabaseObject

            return Project.Items.Where(i => i.ItemIdOrPath.StartsWith(itemIdOrPath, StringComparison.OrdinalIgnoreCase) && i.ItemIdOrPath.IndexOf('/', index) < 0);
        }

        public IXPathItem GetParent()
        {
            var n = ItemPath.LastIndexOf('/');
            if (n <= 0)
            {
                return null;
            }

            var parentItemPath = ItemPath.Left(n);

            var item = Project.FindQualifiedItem(parentItemPath) as IXPathItem;

            return item ?? new XPathItem(Project, parentItemPath);
        }
    }
}
