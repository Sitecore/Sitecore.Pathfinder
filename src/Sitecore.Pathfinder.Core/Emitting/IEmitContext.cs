// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Emitting
{
    public interface IEmitContext
    {
        [NotNull, ItemNotNull]
        ICollection<OutputFile> OutputFiles { get; }

        [NotNull]
        IProjectBase Project { get; }

        [NotNull]
        IProjectEmitter ProjectEmitter { get; }

        [NotNull]
        IEmitContext With([NotNull] IProjectEmitter projectEmitter, [NotNull] IProjectBase project);
    }
}
