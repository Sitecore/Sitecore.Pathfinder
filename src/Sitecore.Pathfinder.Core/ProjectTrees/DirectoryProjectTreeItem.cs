// � 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public class DirectoryProjectTreeItem : ProjectTreeItemBase
    {
        [FactoryConstructor]
        public DirectoryProjectTreeItem([NotNull] IProjectTree projectTree, [NotNull] string directory) : base(projectTree, new ProjectTreeUri(directory))
        {
            Directory = directory;
            Name = Path.GetFileName(Directory);
        }

        [NotNull]
        public string Directory { get; }

        public override string Name { get; }
    }
}
