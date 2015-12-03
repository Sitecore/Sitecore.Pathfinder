// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Axes
{
    public class Self : StepBase
    {
        [CanBeNull]
        private readonly Predicate _predicate;

        public Self([CanBeNull] Predicate predicate)
        {
            _predicate = predicate;
        }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            object result = null;

            Process(xpath, context, _predicate, NextStep, ref result);

            return result;
        }
    }
}
