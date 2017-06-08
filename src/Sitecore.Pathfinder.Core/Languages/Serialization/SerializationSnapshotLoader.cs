// // © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class SerializationSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public SerializationSnapshotLoader([NotNull] IFactory factory)
        {
            Factory = factory;
            Priority = 1000;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanLoad(ISourceFile sourceFile) => string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".item", StringComparison.OrdinalIgnoreCase);

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile) => Factory.SerializationTextSnapshot(sourceFile);
    }
}
