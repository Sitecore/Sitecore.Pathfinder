// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Self : Step
    {
        [CanBeNull]
        private readonly Predicate _predicate;

        public Self([CanBeNull] Predicate predicate)
        {
            _predicate = predicate;
        }

        public override object Evaluate(Query query, object context)
        {
            object result = null;

            Process(query, context, _predicate, NextStep, ref result);

            return result;
        }
    }
}
