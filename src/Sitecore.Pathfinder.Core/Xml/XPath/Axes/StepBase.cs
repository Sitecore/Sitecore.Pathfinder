// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public abstract class StepBase : Opcode
    {
        [CanBeNull]
        public StepBase NextStep { get; set; }

        protected bool Break([NotNull] XPathExpression xpath, [CanBeNull] object context)
        {
            if (xpath.Abort)
            {
                return true;
            }

            if (context == null)
            {
                return false;
            }

            return xpath.AnyCounter > 0;
        }
    }
}
