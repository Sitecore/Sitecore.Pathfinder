// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitters
{
    [InheritedExport]
    public interface IEmitter
    {
        double Sortorder { get; }

        bool CanEmit([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem);

        void Emit([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem);
    }
}
