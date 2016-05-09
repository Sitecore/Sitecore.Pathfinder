// © 2016 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Checking
{
    public abstract class CheckerBase : IChecker
    {
        protected CheckerBase()
        {
            Name = GetType().Name;
        }

        public string Category { get; protected set; } = string.Empty;

        public string Name { get; }

        public abstract void Check(ICheckerContext context);
    }
}
