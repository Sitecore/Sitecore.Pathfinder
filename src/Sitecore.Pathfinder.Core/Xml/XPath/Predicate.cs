// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Axes;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Predicate : StepBase
    {
        public Predicate([NotNull] string expression)
        {
            var queryParser = new XPathParser();
            Expression = queryParser.ParsePredicate(expression);
        }

        public Predicate([NotNull] Opcode expression)
        {
            Expression = expression;
        }

        [CanBeNull]
        public object Current { get; private set; }

        [NotNull]
        public Opcode Expression { get; }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            return Test(xpath, context);
        }

        public bool Test([NotNull] XPathExpression xpath, [CanBeNull] object result)
        {
            Current = result;

            xpath.PredicateCounter++;
            xpath.AnyCounter++;

            object expressionResult;
            try
            {
                expressionResult = Expression.Evaluate(xpath, result);
            }
            finally
            {
                xpath.PredicateCounter--;
                xpath.AnyCounter--;
            }

            if (expressionResult is bool)
            {
                return (bool)expressionResult;
            }

            if (expressionResult is int)
            {
                return GetPosition(result) == (int)expressionResult;
            }

            return expressionResult != null;
        }

        private int GetPosition([CanBeNull] object result)
        {
            var item = result as IXPathItem;
            if (item == null)
            {
                return -1;
            }

            var parent = item.GetParent();
            if (parent == null)
            {
                return -1;
            }

            var children = parent.GetChildren().ToList();
            var index = children.IndexOf(item);
            return index < 0 ? -1 : index + 1;
        }
    }
}
