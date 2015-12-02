// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
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

        [CanBeNull]
        public override object Evaluate(Query query, object context)
        {
            var arguments = new Opcode[_arguments.Count];

            for (var n = 0; n < _arguments.Count; n++)
            {
                arguments[n] = _arguments[n];
            }

            return query.EvaluateFunction(this, arguments, context);
        }
    }
}
