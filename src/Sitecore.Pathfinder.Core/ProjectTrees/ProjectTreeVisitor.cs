// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProjectTreeVisitor
    {
        public virtual void Visit([NotNull] IProjectTree projectTree, [NotNull, ItemNotNull] ICollection<string> sourceFileNames)
        {
            foreach (var projectTreeItem in projectTree.Roots)
            {
                Visit(sourceFileNames, projectTreeItem);
            }
        }

        protected virtual void Visit([NotNull, ItemNotNull] ICollection<string> sourceFileNames, [NotNull] IProjectTreeItem projectTreeItem)
        {
            var sourceFile = projectTreeItem as IProjectTreeSourceFile;
            if (sourceFile != null)
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
