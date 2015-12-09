// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules.Conditions
{
    public abstract class IntConditionBase : ConditionBase
    {
        protected IntConditionBase([NotNull] string name) : base(name)
        {
        }

        public override bool Evaluate(IRuleContext ruleContext, IDictionary<string, object> parameters)
        {
            var value = GetValue(ruleContext, parameters);

            var val = GetParameterValue(parameters, "value", ruleContext.Object);
            if (val != null)
            {
                return value == Parse(val);
            }

            var equals = GetParameterValue(parameters, "=", ruleContext.Object);
            if (equals != null)
            {
                return value == Parse(equals);
            }

            var notequals = GetParameterValue(parameters, "!=", ruleContext.Object);
            if (notequals != null)
            {
                return value == Parse(notequals);
            }

            var largerThan = GetParameterValue(parameters, ">", ruleContext.Object);
            if (largerThan != null)
            {
                return value > Parse(largerThan);
            }

            var largerThanOrEqual = GetParameterValue(parameters, ">=", ruleContext.Object);
            if (largerThanOrEqual != null)
            {
                return value == Parse(largerThanOrEqual);
            }

            var lessThanOrEqual = GetParameterValue(parameters, "<=", ruleContext.Object);
            if (lessThanOrEqual != null)
            {
                return value == Parse(lessThanOrEqual);
            }

            var lessThan = GetParameterValue(parameters, "<", ruleContext.Object);
            if (lessThan != null)
            {
                return value == Parse(lessThan);
            }

            throw new ConfigurationException(Texts.String_operator_not_found);
        }

        [NotNull]
        protected abstract int GetValue([NotNull] IRuleContext ruleContext, [NotNull] IDictionary<string, object> parameters);

        private static int Parse([NotNull] string value)
        {
            int result;
            if (!int.TryParse(value, out result))
            {
                throw new InvalidOperationException("Cannot parse int:" + value);
            }

            return result;
        }
    }
}
