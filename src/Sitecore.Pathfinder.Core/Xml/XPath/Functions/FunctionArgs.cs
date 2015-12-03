// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath.Functions
{
    public class FunctionArgs : EventArgs
    {
        public FunctionArgs([NotNull] XPathExpression xpath, [NotNull] [ItemNotNull] Opcode[] arguments, [CanBeNull] object context)
        {
            XPathExpression = xpath;
            Context = context;
            Arguments = arguments;
        }

        [NotNull]
        [ItemNotNull]
        public Opcode[] Arguments { get; }

        [CanBeNull]
        public object Context { get; set; }

        [NotNull]
        public XPathExpression XPathExpression { get; }
    }
}
