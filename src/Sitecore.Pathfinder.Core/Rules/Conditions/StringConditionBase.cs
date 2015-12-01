// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public abstract class StringConditionBase : ConditionBase
    {
        protected StringConditionBase([NotNull] string name) : base(name)
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, string> parameters)
        {
            // return false, if no elements
            var result = false;

            foreach (var value in GetValues(ruleContext, parameters))
            {
                if (!CompareValue(value, parameters))
                {
                    return false;
                }

                result = true;
            }

            return result;
        }

        private bool CompareValue([NotNull] string value, [NotNull] IDictionary<string, string> parameters)
        {
            var equals = parameters.GetString("=");
            if (!string.IsNullOrEmpty(@equals))
            {
                return string.Equals(value, @equals, StringComparison.OrdinalIgnoreCase);
            }

            var notequals = parameters.GetString("!=");
            if (!string.IsNullOrEmpty(notequals))
            {
                return !string.Equals(value, notequals, StringComparison.OrdinalIgnoreCase);
            }

            var contains = parameters.GetString("contains");
            if (!string.IsNullOrEmpty(contains))
            {
                return value.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            var startsWith = parameters.GetString("starts-with");
            if (!string.IsNullOrEmpty(startsWith))
            {
                return value.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase);
            }

            var endsWith = parameters.GetString("ends-with");
            if (!string.IsNullOrEmpty(endsWith))
            {
                return value.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase);
            }

            var matches = parameters.GetString("matches");
            if (!string.IsNullOrEmpty(matches))
            {
                return Regex.IsMatch(value, matches, RegexOptions.IgnoreCase);
            }

            throw new ConfigurationException("No string operator found");
        }

        [ItemNotNull]
        [NotNull]
        protected abstract IEnumerable<string> GetValues([NotNull] IRuleContext ruleContext, [NotNull] IDictionary<string, string> parameters);
    }
}
