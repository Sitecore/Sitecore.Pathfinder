// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Xml.XPath.Functions;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public class XPathExpression
    {
        [CanBeNull]
        private Opcode _opcode;

        public XPathExpression([NotNull] IXPathService xpathService)
        {
            XPathService = xpathService;
        }

        public bool Abort { get; set; }

        public bool Any { get; set; }

        public int AnyCounter { get; set; }

        [NotNull]
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public int PredicateCounter { get; set; }

        [NotNull]
        protected IXPathService XPathService { get; }

        [CanBeNull]
        public object Evaluate([CanBeNull] object context)
        {
            if (_opcode == null)
            {
                throw new XPathException("No query.");
            }

            PredicateCounter = 0;
            AnyCounter = Any ? 1 : 0;

            return _opcode.Evaluate(this, context);
        }

        [CanBeNull]
        public object EvaluateFunction([NotNull] Function function, [NotNull] [ItemNotNull] Opcode[] arguments, [CanBeNull] object context)
        {
            var func = XPathService.Functions.FirstOrDefault(f => string.Equals(f.Name, function.Name, StringComparison.OrdinalIgnoreCase));
            if (func == null)
            {
                throw new XPathException("Function not found: " + function.Name);
            }

            var args = new FunctionArgs(this, arguments, context);
            return func.Evaluate(args);
        }

        [NotNull]
        public XPathExpression Parse([NotNull] string query)
        {
            var queryParser = new XPathParser();

            _opcode = queryParser.Parse(query);

            return this;
        }
    }
}
