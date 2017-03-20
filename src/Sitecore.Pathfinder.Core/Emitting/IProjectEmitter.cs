// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    public interface IProjectEmitter
    {
        bool CanEmit([NotNull] string format);

        void Emit([NotNull] IProject project);
    }
}
