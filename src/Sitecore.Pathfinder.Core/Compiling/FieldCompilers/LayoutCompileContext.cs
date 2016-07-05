// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LayoutCompileContext
    {
        public LayoutCompileContext([NotNull] ITraceService trace, [NotNull] IProjectBase project, [NotNull] Database database, [NotNull] ITextSnapshot snapshot)
        {
            Trace = trace;
            Project = project;
            Database = database;
            Snapshot = snapshot;
        }

        [NotNull]
        public Database Database { get; }

        [NotNull]
        public IProjectBase Project { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }

        [NotNull]
        public ITraceService Trace { get; }
    }
}
