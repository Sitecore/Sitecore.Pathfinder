// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Emitters;

namespace Sitecore.Pathfinder.Builders.FieldResolvers.Layouts
{
    public class LayoutResolveContext
    {
        public LayoutResolveContext([Diagnostics.NotNull] IEmitContext emitContext, [Diagnostics.NotNull] ITextSnapshot snapshot, [Diagnostics.NotNull] string databaseName)
        {
            EmitContext = emitContext;
            Snapshot = snapshot;
            DatabaseName = databaseName;
        }

        [Diagnostics.NotNull]
        public string DatabaseName { get; }

        [Diagnostics.NotNull]
        public IEmitContext EmitContext { get; }

        [Diagnostics.NotNull]
        public ITextSnapshot Snapshot { get; }
    }
}
