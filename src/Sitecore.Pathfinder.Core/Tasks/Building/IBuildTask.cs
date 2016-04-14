// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public interface IBuildTask : ITask
    {
        void Run([NotNull] IBuildContext context);
    }
}
