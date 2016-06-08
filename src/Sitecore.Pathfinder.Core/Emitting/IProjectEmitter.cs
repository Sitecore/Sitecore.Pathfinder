// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    public interface IProjectEmitter
    {
        void Emit([NotNull] IProject project);
    }
}
