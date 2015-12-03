// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class ItemElement : ElementBase
    {
        public ItemElement([NotNull] string name, [CanBeNull] Predicate predicate) : base(name)
        {
            Predicate = predicate;
        }

        [CanBeNull]
        public Predicate Predicate { get; }
    }
}
