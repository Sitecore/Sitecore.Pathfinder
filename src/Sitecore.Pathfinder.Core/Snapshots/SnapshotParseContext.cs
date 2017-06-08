// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    public class SnapshotParseContext
    {
        [NotNull]
        public static readonly SnapshotParseContext Empty = new SnapshotParseContext(Projects.Project.Empty, new Dictionary<string, string>());

        [FactoryConstructor]
        public SnapshotParseContext([NotNull] IProjectBase project, [NotNull] IDictionary<string, string> tokens)
        {
            Project = project;
            Tokens = tokens;
        }

        [NotNull]
        public IProjectBase Project { get; }

        [NotNull]
        public IDictionary<string, string> Tokens { get; }
    }
}
