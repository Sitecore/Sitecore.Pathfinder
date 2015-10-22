// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public class SnapshotParseContext
    {
        [NotNull]
        public static readonly SnapshotParseContext Empty = new SnapshotParseContext(new Dictionary<string, string>(), new Dictionary<string, ITextNode>());

        public SnapshotParseContext([NotNull] IDictionary<string, string> tokens, [NotNull] IDictionary<string, ITextNode> innerTextNodes)
        {
            InnerTextNodes = innerTextNodes;
            Tokens = tokens;
        }

        [NotNull]
        public IDictionary<string, ITextNode> InnerTextNodes { get; }

        [NotNull]
        public IDictionary<string, string> Tokens { get; }
    }
}
