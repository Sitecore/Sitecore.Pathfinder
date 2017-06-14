// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LayoutCompileContext
    {
        [FactoryConstructor]
        public LayoutCompileContext([NotNull] IProjectBase project, [NotNull] IDatabase database, [NotNull] ITextSnapshot snapshot)
        {
            Project = project;
            Database = database;
            Snapshot = snapshot;
        }

        [NotNull]
        public IDatabase Database { get; }

        [NotNull]
        public IProjectBase Project { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }
    }
}
