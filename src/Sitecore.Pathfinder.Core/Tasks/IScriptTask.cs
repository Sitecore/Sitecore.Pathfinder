// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Tasks
{
    public interface IScriptTask : ITask
    {
        [NotNull]
        ITask With([NotNull] string script);
    }
}
