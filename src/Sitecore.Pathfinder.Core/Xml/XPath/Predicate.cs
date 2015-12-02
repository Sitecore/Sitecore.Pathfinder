// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class Predicate : Step
    {
        public Predicate([NotNull] string expression)
        {
            var queryParser = new QueryParser();
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

        public override object Evaluate(Query query, object context)
        {
            return Test(query, context);
        }

        public bool Test([CanBeNull] object result)
        {
            var query = new Query(this);

            query.BeginQuery();

            return Test(query, result);
        }

        public bool Test([NotNull] Query query, [CanBeNull] object result)
        {
            Current = result;

            query.PredicateCounter++;
            query.AnyCounter++;

            object expressionResult;
            try
            {
                expressionResult = Expression.Evaluate(query, result);
            }
            finally
            {
                query.PredicateCounter--;
                query.AnyCounter--;
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

        int GetPosition([CanBeNull] object result)
        {
            var item = result as Item;
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
