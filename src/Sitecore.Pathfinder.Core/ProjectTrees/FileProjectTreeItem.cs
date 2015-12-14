// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public class FileProjectTreeItem : ProjectTreeItemBase
    {
        public FileProjectTreeItem([NotNull] ProjectTreeUri uri) : base(uri)
        {
            Name = Path.GetFileNameWithoutExtension(uri.Uri);
        }

        public override string Name { get; }

        public override IProjectTreeItem ParentItem { get; }

        public override IEnumerable<IProjectTreeItem> GetChildren()
        {
            yield break;
        }
    }
}