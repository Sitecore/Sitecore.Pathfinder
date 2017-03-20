// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting
{
    public interface IEmitter
    {
        double Sortorder { get; }

        bool CanEmit([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem);

        void Emit([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem);
    }
}
