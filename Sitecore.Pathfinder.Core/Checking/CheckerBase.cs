// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Checking
{
    public abstract class CheckerBase : IChecker
    {
        public abstract void Check(ICheckerContext context);
    }
}
