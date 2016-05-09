// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ValidFromValidToChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                if (item.Publishing.NeverPublish)
                {
                    continue;
                }

                var validFrom = item.Publishing.ValidFrom;
                var validTo = item.Publishing.ValidTo;

                if (validFrom != DateTime.MinValue && validTo != DateTime.MinValue && validFrom > validTo)
                {
                    context.Trace.TraceWarning(Msg.C1021, "Valid From/Valid To dates", TraceHelper.GetTextNode(item.Fields[Constants.Fields.ValidFrom], item.Fields[Constants.Fields.ValidTo], item), "The Valid From date is after the Valid To date. Change either the Valid From date or the Valid To date.");
                }
            }
        }
    }
}
