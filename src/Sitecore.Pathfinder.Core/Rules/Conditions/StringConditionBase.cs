// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public abstract class StringConditionBase : ConditionBase
    {
        protected StringConditionBase([NotNull] string name) : base(name)
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var value = GetValue(ruleContext, parameters);

            var val = GetParameterValue(parameters, "value", ruleContext.Object);
            if (val != null)
            {
                return string.Equals(value, val, StringComparison.OrdinalIgnoreCase);
            }

            var equals = GetParameterValue(parameters, "=", ruleContext.Object);
            if (equals != null)
            {
                return string.Equals(value, equals, StringComparison.OrdinalIgnoreCase);
            }

            var notequals = GetParameterValue(parameters, "!=", ruleContext.Object);
            if (notequals != null)
            {
                return !string.Equals(value, notequals, StringComparison.OrdinalIgnoreCase);
            }

            var contains = GetParameterValue(parameters, "contains", ruleContext.Object);
            if (contains != null)
            {
                return value.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            var startsWith = GetParameterValue(parameters, "starts-with", ruleContext.Object);
            if (startsWith != null)
            {
                return value.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase);
            }

            var endsWith = GetParameterValue(parameters, "ends-with", ruleContext.Object);
            if (endsWith != null)
            {
                return value.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase);
            }

            var matches = GetParameterValue(parameters, "matches", ruleContext.Object);
            if (matches != null)
            {
                return Regex.IsMatch(value, matches, RegexOptions.IgnoreCase);
            }

            throw new ConfigurationException(Texts.String_operator_not_found);
        }

        [NotNull]
        protected abstract string GetValue([NotNull] IRuleContext ruleContext, [NotNull] IDictionary<string, object> parameters);
    }
}
