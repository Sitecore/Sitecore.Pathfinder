// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath;

namespace Sitecore.Pathfinder.Rules
{
    public class ConditionActionBase
    {
        [CanBeNull]
        protected virtual string GetParameterValue([NotNull] IDictionary<string, object> parameters, [NotNull] string key, [CanBeNull] object context)
        {
            object value;
            if (!parameters.TryGetValue(key, out value))
            {
                return null;
            }

            var xpathExpression = value as XPathExpression;
            if (xpathExpression != null)
            {
                value = xpathExpression.Evaluate(context);
            }

            return value?.ToString();
        }
    }
}
