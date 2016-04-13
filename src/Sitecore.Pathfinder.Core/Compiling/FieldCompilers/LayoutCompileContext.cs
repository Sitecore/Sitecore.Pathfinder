﻿// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.FieldCompilers
{
    public class LayoutCompileContext
    {
        public LayoutCompileContext([NotNull] ITraceService trace, [NotNull] IProject project, [NotNull] ITextSnapshot snapshot)
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
