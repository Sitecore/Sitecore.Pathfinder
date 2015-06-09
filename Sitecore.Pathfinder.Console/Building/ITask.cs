// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    public interface ITask
    {
        [NotNull]
        string TaskName { get; }

        void Run([NotNull] IBuildContext context);
    }
}
