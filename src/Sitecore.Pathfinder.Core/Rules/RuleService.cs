// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Rules.Conditions;
using Sitecore.Pathfinder.Rules.Conditions.LogicConditions;
using Sitecore.Pathfinder.Rules.Conditions.XPathConditions;
using Sitecore.Pathfinder.Xml.XPath;

namespace Sitecore.Pathfinder.Rules
{
    [Export(typeof(IRuleService))]
    public class RuleService : IRuleService
    {
        [NotNull]
        private static readonly Dictionary<string, object> EmptyParameters = new Dictionary<string, object>();

        [ImportingConstructor]
        public RuleService([NotNull] IConfiguration configuration, [NotNull] IXPathService xpathService, [ImportMany, NotNull, ItemNotNull] IEnumerable<ICondition> conditions, [ImportMany, NotNull, ItemNotNull] IEnumerable<IAction> actions)
        {
            Configuration = configuration;
            XPathService = xpathService;
            Conditions = conditions;
            Actions = actions;
        }

        public IEnumerable<IAction> Actions { get; }

        public IEnumerable<ICondition> Conditions { get; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IXPathService XPathService { get; }

        public IRule ParseRule(string configurationKey)
        {
            var filter = Configuration.GetString(configurationKey + ":filter");

            var rule = new Rule(filter);

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
        protected virtual RuleCondition ParseConditionAnd([NotNull] string configurationKey, [NotNull] string key)
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
        protected virtual RuleCondition ParseConditionNot([NotNull] string configurationKey, [NotNull] string key)
        {
            var pair = Configuration.GetSubKeys(configurationKey + ":" + key).FirstOrDefault();

            var ruleCondition = ParseCondition(configurationKey + ":" + key, pair.Key);

            return new RuleCondition(new NotCondition(ruleCondition), EmptyParameters);
        }

        [NotNull]
        protected virtual RuleCondition ParseConditionEvalXPath([NotNull] string configurationKey, [NotNull] string key)
        {
            var xpath = Configuration.GetString(configurationKey + ":" + key);

            var xpathExpression = XPathService.GetXPathExpression(xpath);

            return new RuleCondition(new XPathCondition(xpathExpression), EmptyParameters);
        }

        [NotNull]
        protected virtual RuleCondition ParseConditionOr([NotNull] string configurationKey, [NotNull] string key)
        {
            var conditions = new List<RuleCondition>();

            foreach (var pair in Configuration.GetSubKeys(configurationKey + ":" + key))
            {
                var ruleCondition = ParseCondition(configurationKey + ":" + key, pair.Key);
                conditions.Add(ruleCondition);
            }

            return new RuleCondition(new OrCondition(conditions), EmptyParameters);
        }

        protected virtual void ParseElse([NotNull] Rule rule, [NotNull] string configurationKey)
        {
            var key = configurationKey + ":else";

            foreach (var pair in Configuration.GetSubKeys(key))
            {
                var ruleAction = ParseAction(key, pair.Key);
                rule.Else.Add(ruleAction);
            }
        }

        protected virtual void ParseIf([NotNull] Rule rule, [NotNull] string configurationKey)
        {
            var key = configurationKey + ":if";

            foreach (var pair in Configuration.GetSubKeys(key))
            {
                var ruleCondition = ParseCondition(key, pair.Key);
                rule.If.Add(ruleCondition);
            }
        }

        [NotNull]
        protected virtual Dictionary<string, object> ParseParameters([NotNull] string configurationKey, [NotNull] string key)
        {
            var parameter = new Dictionary<string, object>();

            var keys = Configuration.GetSubKeys(configurationKey + ":" + key);
            if (keys.Any())
            {
                foreach (var pair in keys)
                {
                    parameter[pair.Key] = ParseParameterValue(Configuration.GetString(configurationKey + ":" + key + ":" + pair.Key));
                }
            }
            else
            {
                parameter["value"] = ParseParameterValue(Configuration.GetString(configurationKey + ":" + key));
            }

            return parameter;
        }

        [NotNull]
        protected virtual object ParseParameterValue([NotNull] string value)
        {
            // todo: make this pluggable
            if (value.StartsWith("xpath:", StringComparison.OrdinalIgnoreCase))
            {
                return XPathService.GetXPathExpression(value.Mid(6).Trim());
            }

            return value;
        }

        protected virtual void ParseThen([NotNull] Rule rule, [NotNull] string configurationKey)
        {
            var key = configurationKey + ":then";

            foreach (var pair in Configuration.GetSubKeys(key))
            {
                var ruleAction = ParseAction(key, pair.Key);
                rule.Then.Add(ruleAction);
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

            throw new ConfigurationException(Texts.Action_not_found__ + key);
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

            if (key == "eval-xpath" || key.StartsWith("eval-xpath-"))
            {
                return ParseConditionEvalXPath(configurationKey, key);
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

            throw new ConfigurationException(Texts.Condition_not_found__ + key);
        }
    }
}
