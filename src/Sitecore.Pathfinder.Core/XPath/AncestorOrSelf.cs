// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.XPath
{
    public class AncestorOrSelf : Ancestor
    {
        public AncestorOrSelf([NotNull] ElementBase element) : base(element)
        {
        }

        protected override Item GetContextNode(object context)
        {
            return context as Item;
        }
    }
}
