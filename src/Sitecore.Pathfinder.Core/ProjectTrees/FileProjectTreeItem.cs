// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public class FileProjectTreeItem : ProjectTreeItemBase, IProjectTreeSourceFile
    {
        public FileProjectTreeItem([NotNull] IProjectTree projectTree, [NotNull] string fileName) : base(projectTree, new ProjectTreeUri(fileName))
        {
            FileName = fileName;
            Name = Path.GetFileNameWithoutExtension(fileName);
        }

        public string FileName { get; }

        public override string Name { get; }
    }
}
