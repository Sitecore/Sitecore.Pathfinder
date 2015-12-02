// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
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

        public void BeginQuery()
        {
            PredicateCounter = 0;
            AnyCounter = Any ? 1 : 0;
        }

        [CanBeNull]
        public object Evaluate([CanBeNull] object context)
        {
            if (_opcode == null)
            {
                throw new QueryException("No query.");
            }

            BeginQuery();

            return _opcode.Evaluate(this, context);
        }

        [CanBeNull]
        public object EvaluateFunction([NotNull] Function function, [NotNull] [ItemNotNull] Opcode[] arguments, [CanBeNull] object context)
        {
            var args = new FunctionArgs(this, function, arguments, context);
            return Function?.Invoke(args);
        }

        public event FunctionEventHandler Function;

        public void Parse([NotNull] string query)
        {
            var queryParser = new QueryParser();
            _opcode = queryParser.Parse(query);
        }
    }
}
