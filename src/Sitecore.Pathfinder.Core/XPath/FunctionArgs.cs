// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class FunctionArgs : EventArgs
    {
        public FunctionArgs([NotNull] Query query, [NotNull] Function function, [NotNull] [ItemNotNull] Opcode[] arguments, [CanBeNull] object context)
        {
            Query = query;
            FunctionName = function.Name;
            Function = function;
            Context = context;
            Arguments = arguments;
        }

        [NotNull]
        [ItemNotNull]
        public Opcode[] Arguments { get; }

        [NotNull]
        public Function Function { get; }

        [NotNull]
        public string FunctionName { get; }

        [NotNull]
        public Query Query { get; }

        [CanBeNull]
        public object Context { get; set; }
    }
}
