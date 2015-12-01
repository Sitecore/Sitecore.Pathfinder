// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Rules
{
    [Export(typeof(IRuleService))]
    public class RuleService : IRuleService
    {
        [NotNull]
        private static readonly Dictionary<string, string> EmptyParameters = new Dictionary<string, string>();

        [ImportingConstructor]
        public RuleService([NotNull] IConfiguration configuration, [ImportMany] [NotNull] [ItemNotNull] IEnumerable<ICondition> conditions, [ImportMany] [NotNull] [ItemNotNull] IEnumerable<IAction> actions)
        {
            Configuration = configuration;
            Conditions = conditions;
            Actions = actions;
        }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<IAction> Actions { get; }

        [NotNull]
        [ItemNotNull]
        protected IEnumerable<ICondition> Conditions { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public IRule ParseRule(string configurationKey)
        {
            var rule = new Rule();

            ParseIf(rule, configurationKey);
            ParseThen(rule, configurationKey);
            ParseElse(rule, configurationKey);

            return rule;
        }

        public IEnumerable<IRule> ParseRules(string configurationKey)
        {
            foreach (var pair in Configuration.GetSubKeys(configurationKey))
            {
                yield return ParseRule(configurationKey + ":" + pair.Key);
            }
        }

        [NotNull]
        private RuleAction ParseAction([NotNull] string configurationKey, [NotNull] string key)
        {
            foreach (var action in Actions)
            {
                if (!key.StartsWith(action.Name))
                {
                    continue;
                }

                var parameters = ParseParameters(configurationKey, key);
                return new RuleAction(action, parameters);
            }

            throw new ConfigurationException("Action not found: " + key);
        }

        [NotNull]
        private RuleCondition ParseCondition([NotNull] string configurationKey, [NotNull] string key)
        {
            if (key == "and" || key.StartsWith("and-"))
            {
                return ParseConditionAnd(configurationKey, key);
            }

            if (key == "or" || key.StartsWith("or-"))
            {
                return ParseConditionOr(configurationKey, key);
            }

            if (key == "not" || key.StartsWith("not-"))
            {
                return ParseConditionNot(configurationKey, key);
            }

            foreach (var condition in Conditions)
            {
                if (!key.StartsWith(condition.Name))
                {
                    continue;
                }

                var parameters = ParseParameters(configurationKey, key);
                return new RuleCondition(condition, parameters);
            }

            throw new ConfigurationException("Condition not found: " + key);
        }

        [NotNull]
        private RuleCondition ParseConditionAnd([NotNull] string configurationKey, [NotNull] string key)
        {
            var conditions = new List<RuleCondition>();

            foreach (var pair in Configuration.GetSubKeys(configurationKey + ":" + key))
            {
                var ruleCondition = ParseCondition(configurationKey + ":" + key, pair.Key);
                conditions.Add(ruleCondition);
            }

            return new RuleCondition(new AndCondition(conditions), EmptyParameters);
        }

        [NotNull]
        private RuleCondition ParseConditionNot([NotNull] string configurationKey, [NotNull] string key)
        {
            var pair = Configuration.GetSubKeys(configurationKey + ":" + key).FirstOrDefault();

            var ruleCondition = ParseCondition(configurationKey + ":" + key, pair.Key);

            return new RuleCondition(new NotCondition(ruleCondition), EmptyParameters);
        }

        [NotNull]
        private RuleCondition ParseConditionOr([NotNull] string configurationKey, [NotNull] string key)
        {
            var conditions = new List<RuleCondition>();

            foreach (var pair in Configuration.GetSubKeys(configurationKey + ":" + key))
            {
                var ruleCondition = ParseCondition(configurationKey + ":" + key, pair.Key);
                conditions.Add(ruleCondition);
            }

            return new RuleCondition(new OrCondition(conditions), EmptyParameters);
        }

        private void ParseElse([NotNull] Rule rule, [NotNull] string configurationKey)
        {
            var key = configurationKey + ":else";

            foreach (var pair in Configuration.GetSubKeys(key))
            {
                var ruleAction = ParseAction(key, pair.Key);
                rule.Else.Add(ruleAction);
            }
        }

        private void ParseIf([NotNull] Rule rule, [NotNull] string configurationKey)
        {
            var key = configurationKey + ":if";

            foreach (var pair in Configuration.GetSubKeys(key))
            {
                var ruleCondition = ParseCondition(key, pair.Key);
                rule.If.Add(ruleCondition);
            }
        }

        [NotNull]
        private Dictionary<string, string> ParseParameters([NotNull] string configurationKey, [NotNull] string key)
        {
            var parameter = new Dictionary<string, string>();

            foreach (var pair in Configuration.GetSubKeys(configurationKey + ":" + key))
            {
                parameter[pair.Key] = Configuration.GetString(configurationKey + ":" + key + ":" + pair.Key);
            }

            return parameter;
        }

        private void ParseThen([NotNull] Rule rule, [NotNull] string configurationKey)
        {
            var key = configurationKey + ":then";

            foreach (var pair in Configuration.GetSubKeys(key))
            {
                var ruleAction = ParseAction(key, pair.Key);
                rule.Then.Add(ruleAction);
            }
        }
    }
}
