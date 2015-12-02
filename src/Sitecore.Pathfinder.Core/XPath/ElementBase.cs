// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public abstract class ElementBase : Step
    {
        protected ElementBase([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public string Name { get; }
    }
}
