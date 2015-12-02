// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public abstract class Step : Opcode
    {
        [CanBeNull]
        public Step NextStep { get; set; }

        protected bool Break([NotNull] Query query, [CanBeNull] object context)
        {
            if (query.Abort)
            {
                return true;
            }

            if (context == null)
            {
                return false;
            }

            return query.AnyCounter > 0;
        }
    }
}
