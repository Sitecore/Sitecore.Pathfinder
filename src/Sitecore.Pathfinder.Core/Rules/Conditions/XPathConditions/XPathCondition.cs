// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;
using Sitecore.Pathfinder.Xml.XPath;

namespace Sitecore.Pathfinder.Rules.Conditions.XPathConditions
{
    [PartNotDiscoverable]
    public class XPathCondition : ConditionBase
    {
        public XPathCondition([NotNull, ItemNotNull] XPathExpression xpathExpression) : base("eval-xpath")
        {
            XPathExpression = xpathExpression;
        }

        [NotNull]
        protected XPathExpression XPathExpression { get; }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var result = XPathExpression.Evaluate(ruleContext.Object);

            if (result == null)
            {
                return false;
            }

            if (result is bool)
            {
                return (bool)result;
            }

            if (result is int)
            {
                return (int)result != 0;
            }

            if (result is string)
            {
                return !string.IsNullOrEmpty(result as string);
            }

            return true;
        }
    }
}
