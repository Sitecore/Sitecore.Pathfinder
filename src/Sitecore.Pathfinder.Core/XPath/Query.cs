// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public delegate object FunctionEventHandler([NotNull] FunctionArgs args);

    public delegate void MatchEventHandler([NotNull] MatchArgs args);

    public class Query
    {
        [CanBeNull]
        private Opcode _opcode;

        public Query()
        {
            Function += Functions.FunctionCall;
        }

        public Query([NotNull] Opcode opcode)
        {
            _opcode = opcode;
            Function += Functions.FunctionCall;
        }

        public bool Abort { get; set; }

        public bool Any { get; set; }

        [NotNull]
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        internal int AnyCounter { get; set; }

        internal int PredicateCounter { get; set; }

        [CanBeNull]
        public object Execute([NotNull] string query)
        {
            return Execute(query, null);
        }

        [CanBeNull]
        public object Execute([NotNull] string query, [CanBeNull] object context)
        {
            var opcode = QueryParser.Parse(query);

            BeginQuery();

            return opcode.Evaluate(this, context);
        }

        [CanBeNull]
        public object Evaluate([NotNull] string query)
        {
            return Evaluate(query, null);
        }

        [CanBeNull]
        public object Evaluate([NotNull] string query, [CanBeNull] object context)
        {
            var opcode = QueryParser.ParseExpression(query);

            BeginQuery();

            return opcode.Evaluate(this, context);
        }

        public event FunctionEventHandler Function;

        public event MatchEventHandler Match;

        public void BeginQuery()
        {
            PredicateCounter = 0;
            AnyCounter = (Any ? 1 : 0);
        }

        [CanBeNull]
        public object EvaluateFunction([NotNull] Function function, [NotNull] [ItemNotNull] Opcode[] arguments, [CanBeNull] object context)
        {
            var args = new FunctionArgs(this, function, arguments, context);
            return Function?.Invoke(args);
        }

        public bool DoMatch([CanBeNull] object context)
        {
            if (Match == null)
            {
                return true;
            }

            var args = new MatchArgs(context);

            Match(args);

            Abort = args.Abort;

            return args.IsMatch;
        }
    }
}
