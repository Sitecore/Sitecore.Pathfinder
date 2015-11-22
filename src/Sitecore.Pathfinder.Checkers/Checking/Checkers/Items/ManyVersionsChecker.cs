// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;

namespace Sitecore.Pathfinder.Checking.Checkers.Items
{
    public class ManyVersionsChecker : CheckerBase
    {
        public ManyVersionsChecker() : base("Items with many versions", Items)
        {
        }

        public override void Check(ICheckerContext context)
        {
            foreach (var item in context.Project.Items)
            {
                foreach (var language in item.GetLanguages())
                {
                    var count = item.GetVersions(language).Count();
                    if (count >= 10)
                    {
                        context.Trace.TraceWarning("Item has many version", $"The item has {count} versions in the {language} language. Items with more than 10 version decrease performance. Remove some of the older versions.");
                    }
                }
            }
        }
    }
}
