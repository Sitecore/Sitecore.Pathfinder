// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public class SnapshotParseContext
    {
        [NotNull]
        public static readonly SnapshotParseContext Empty = new SnapshotParseContext(null, new Dictionary<string, string>(), new Dictionary<string, List<ITextNode>>());

        public SnapshotParseContext([NotNull] ISnapshotService snapshotService, [NotNull] IDictionary<string, string> tokens, [NotNull] IDictionary<string, List<ITextNode>> placeholderTextNodes)
        {
            SnapshotService = snapshotService;
            PlaceholderTextNodes = placeholderTextNodes;
            Tokens = tokens;
        }

        [NotNull]
        public IDictionary<string, List<ITextNode>> PlaceholderTextNodes { get; }

        [NotNull]
        public IDictionary<string, string> Tokens { get; }

        [NotNull]
        public ISnapshotService SnapshotService { get;  }
    }
}
