// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public interface IProjectTreeSourceFile
    {
        [NotNull]
        string FileName { get; }
    }
}
