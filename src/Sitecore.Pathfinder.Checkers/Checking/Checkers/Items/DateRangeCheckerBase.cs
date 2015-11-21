// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public abstract class DateRangeCheckerBase : CheckerBase
    {
        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                Compare(context, item);
            }
        }

        protected abstract void Compare(ICheckerContext context, Item item);

        protected virtual bool CompareDates(Item item, string fieldName0, string fieldName1)
        {
            var value0 = item[fieldName0];
            var value1 = item[fieldName1];

            if (string.IsNullOrEmpty(value0) || string.IsNullOrEmpty(value1))
            {
                return true;
            }

            var date0 = value0.FromIsoToDateTime();
            var date1 = value1.FromIsoToDateTime();

            return date0 <= date1;
        }
    }
}
