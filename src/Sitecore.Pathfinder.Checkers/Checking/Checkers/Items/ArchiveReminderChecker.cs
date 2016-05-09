// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ArchiveReminderChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                var archiveDate = item[Constants.Fields.ArchiveDate].FromIsoToDateTime();
                var reminderDate = item[Constants.Fields.ReminderDate].FromIsoToDateTime();

                if (reminderDate != DateTime.MinValue && archiveDate != DateTime.MinValue && reminderDate > archiveDate)
                {
                    context.Trace.TraceWarning(Msg.C1002, "Reminder/Archive dates", TraceHelper.GetTextNode(item.Fields[Constants.Fields.ArchiveDate], item.Fields[Constants.Fields.ReminderDate], item), "The Reminder date is after the Archive date. Change either the Reminder date or the Archive date.");
                }
            }
        }
    }
}
