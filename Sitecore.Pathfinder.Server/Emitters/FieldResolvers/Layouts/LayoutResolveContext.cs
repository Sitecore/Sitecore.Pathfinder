// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Emitters.FieldResolvers.Layouts
{
    public class LayoutResolveContext
    {
        public LayoutResolveContext(ITraceService trace, [NotNull] IProject project, [NotNull] ITextSnapshot snapshot, string databaseName)
        {
            Trace = trace;
            Project = project;
            Snapshot = snapshot;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public IProject Project { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }

        [NotNull]
        public ITraceService Trace { get; }
    }
}
