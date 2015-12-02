// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.XPath
{
    public class Root : Step
    {
        public override object Evaluate(Query query, object context)
        {
            object result = null;

            Process(query, context, null, NextStep, ref result);

            return result;
        }
    }
}
