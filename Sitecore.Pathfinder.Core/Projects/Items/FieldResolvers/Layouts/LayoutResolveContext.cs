// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Items.FieldResolvers.Layouts
{
    public class LayoutResolveContext
    {
        public LayoutResolveContext(ITraceService trace, [NotNull] IProject project, [NotNull] ITextSnapshot snapshot)
        {
            Trace = trace;
            Project = project;
            Snapshot = snapshot;
        }

        [NotNull]
        public IProject Project { get; }

        [NotNull]
        public ITextSnapshot Snapshot { get; }

        [NotNull]
        public ITraceService Trace { get; }
    }
}
