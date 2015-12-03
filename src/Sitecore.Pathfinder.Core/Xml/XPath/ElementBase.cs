// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Axes;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public abstract class ElementBase : StepBase
    {
        protected ElementBase([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public string Name { get; }
    }
}
