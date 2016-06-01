// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Emitting
{
    public interface IEmitterService
    {
        int Start();

        [NotNull]
        IEmitterService With([NotNull] string baseDirectory);
    }
}
