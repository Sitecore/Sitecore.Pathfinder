// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Checking
{
    public interface IChecker
    {
        void Check([NotNull] ICheckerContext context);
    }
}
