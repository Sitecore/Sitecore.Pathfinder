// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class Function : Opcode
    {
        [NotNull]
        [ItemNotNull]
        private readonly List<Opcode> _arguments = new List<Opcode>();

        public Function([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public string Name { get; }

        public void Add([NotNull] Opcode argument)
        {
            _arguments.Add(argument);
        }

        public override object Evaluate(XPathExpression xpath, object context)
        {
            return xpath.EvaluateFunction(this, _arguments.ToArray(), context);
        }
    }
}
