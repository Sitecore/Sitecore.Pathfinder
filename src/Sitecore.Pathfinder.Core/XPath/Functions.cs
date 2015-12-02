// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.XPath
{
    public class Functions
    {
        public static bool CompareCaseInsensitive([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new QueryException("Too many or to few arguments in CompareCaseInsensitive()");
            }

            var a0 = args.Arguments[0].Evaluate(args.Query, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.Query, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new QueryException("String expression expected in CompareCaseInsensitive()");
            }

            return String.Compare(a0.ToString(), a1.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static bool Contains([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new QueryException("Too many or to few arguments in Contains()");
            }

            var a0 = args.Arguments[0].Evaluate(args.Query, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.Query, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new QueryException("String expression expected in Contains()");
            }

            return a0.ToString().IndexOf(a1.ToString(), StringComparison.InvariantCulture) >= 0;
        }

        [CanBeNull]
        public static object Current([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 0)
            {
                throw new QueryException("Too many or to few arguments in Function()");
            }

            return null;
        }

        public static bool EndsWith([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new QueryException("Too many or to few arguments in EndsWith()");
            }

            var a0 = args.Arguments[0].Evaluate(args.Query, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.Query, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new QueryException("String expression expected in EndsWith()");
            }

            return a0.ToString().EndsWith(a1.ToString(), StringComparison.InvariantCulture);
        }

        public static object FunctionCall([NotNull] FunctionArgs args)
        {
            switch (args.FunctionName.ToLowerInvariant())
            {
                case "contains":
                    return Contains(args);
                case "comparecaseinsensitive":
                    return CompareCaseInsensitive(args);
                case "current":
                    return Current(args);
                case "endswith":
                    return EndsWith(args);
                case "last":
                    return Last(args);
                case "not":
                    return Not(args);
                case "number":
                    return Number(args);
                case "position":
                    return Position(args);
                case "startswith":
                    return StartsWith(args);
            }

            return null;
        }

        public static int Last([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 0)
            {
                throw new QueryException("No arguments allowed in last()");
            }

            var item = args.Context as Item;
            if (item == null)
            {
                return -1;
            }

            var parent = item.GetParent();
            if (parent != null)
            {
                return parent.GetChildren().Count();
            }

            return -1;
        }

        public static bool Not([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 1)
            {
                throw new QueryException("Too many or to few arguments in not()");
            }

            var a0 = args.Arguments[0].Evaluate(args.Query, args.Context);

            if (!(a0 is bool))
            {
                throw new QueryException("Boolean expression expected in not()");
            }

            return !((bool)a0);
        }

        public static int Number([NotNull] FunctionArgs args)
        {
            if (args.Arguments.Length != 1)
            {
                throw new QueryException("Too many or to few arguments in number()");
            }

            var a0 = args.Arguments[0].Evaluate(args.Query, args.Context);

            var s = a0 as string;

            if (s != null)
            {
                int i;

                if (!int.TryParse(s, out i))
                {
                    throw new QueryException("Integer expression expected in number()");
                }

                a0 = i;
            }

            if (!(a0 is int))
            {
                throw new QueryException("Integer expression expected in number()");
            }

            return (int)a0;
        }

        public static int Position(FunctionArgs args)
        {
            if (args.Arguments.Length != 0)
            {
                throw new QueryException("No arguments allowed in position()");
            }

            var item = args.Context as Item;
            if (item == null)
            {
                return -1;
            }

            var parent = item.GetParent();
            if (parent == null)
            {
                return -1;
            }

            var children = parent.GetChildren().ToArray();
            for (var n = 0; n < children.Length; n++)
            {
                if (children[n] == args.Context)
                {
                    return n + 1;
                }
            }

            return -1;
        }

        public static bool StartsWith(FunctionArgs args)
        {
            if (args.Arguments.Length != 2)
            {
                throw new QueryException("Too many or to few arguments in StartsWith()");
            }

            var a0 = args.Arguments[0].Evaluate(args.Query, args.Context);
            var a1 = args.Arguments[1].Evaluate(args.Query, args.Context);

            if (!(a0 is string && a1 is string))
            {
                throw new QueryException("String expression expected in StartsWith()");
            }

            return a0.ToString().StartsWith(a1.ToString(), StringComparison.InvariantCulture);
        }
    }
}
