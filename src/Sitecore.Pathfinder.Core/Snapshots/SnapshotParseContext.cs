// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    public class SnapshotParseContext
    {
        [NotNull]
        public static readonly SnapshotParseContext Empty = new SnapshotParseContext(null, Projects.Project.Empty, new Dictionary<string, string>(), new Dictionary<string, List<ITextNode>>());

        public SnapshotParseContext([NotNull] ISnapshotService snapshotService, [NotNull] IProjectBase project, [NotNull] IDictionary<string, string> tokens, [NotNull] IDictionary<string, List<ITextNode>> placeholderTextNodes)
        {
            SnapshotService = snapshotService;
            Project = project;
            PlaceholderTextNodes = placeholderTextNodes;
            Tokens = tokens;
        }

        [NotNull]
        public IDictionary<string, List<ITextNode>> PlaceholderTextNodes { get; }

        [NotNull]
        public IProjectBase Project { get; }

        [NotNull]
        public ISnapshotService SnapshotService { get; }

        [NotNull]
        public IDictionary<string, string> Tokens { get; }
    }
}
