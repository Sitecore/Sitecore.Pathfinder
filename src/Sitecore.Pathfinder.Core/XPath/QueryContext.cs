// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.XPath
{
    public class QueryContext
    {
        public QueryContext()
        {
        }

        public QueryContext([NotNull] object contextNode)
        {
            ContextNode = contextNode;
        }

        [CanBeNull]
        public object ContextNode { get; }
    }
}
