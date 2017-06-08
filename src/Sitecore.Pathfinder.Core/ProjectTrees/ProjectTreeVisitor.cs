// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public class ProjectTreeVisitor
    {
        [FactoryConstructor]
        public ProjectTreeVisitor()
        {
        }

        public virtual void Visit([NotNull] IProjectTree projectTree, [NotNull, ItemNotNull] ICollection<string> sourceFileNames)
        {
            foreach (var projectTreeItem in projectTree.Roots)
            {
                Visit(sourceFileNames, projectTreeItem);
            }
        }

        protected virtual void Visit([NotNull, ItemNotNull] ICollection<string> sourceFileNames, [NotNull] IProjectTreeItem projectTreeItem)
        {
            if (projectTreeItem is IProjectTreeSourceFile sourceFile)
            {
                sourceFileNames.Add(sourceFile.FileName);
            }

            foreach (var child in projectTreeItem.GetChildren())
            {
                Visit(sourceFileNames, child);
            }
        }
    }
}
