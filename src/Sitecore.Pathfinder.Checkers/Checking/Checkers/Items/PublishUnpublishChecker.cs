// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class PublishUnpublishChecker : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                if (item.Publishing.NeverPublish)
                {
                    continue;
                }

                var publishDate = item.Publishing.PublishDate;
                var unpublishDate = item.Publishing.PublishDate;

                if (publishDate != DateTime.MinValue && unpublishDate != DateTime.MinValue && publishDate > unpublishDate)
                {
                    context.Trace.TraceWarning(Msg.C1011, "Publish/Unpublish dates", TraceHelper.GetTextNode(item.Fields[Constants.Fields.PublishDate], item.Fields[Constants.Fields.UnpublishDate], item), "The Publish date is after the Unpublish date. Change either the Publish date or the Unpublish date.");
                }
            }
        }
    }
}
