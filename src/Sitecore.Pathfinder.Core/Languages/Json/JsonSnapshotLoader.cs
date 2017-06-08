// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class JsonSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public JsonSnapshotLoader([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanLoad(ISourceFile sourceFile) => string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".json", StringComparison.OrdinalIgnoreCase);

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            var contents = sourceFile.ReadAsText(snapshotParseContext.Tokens);

            return Factory.JsonTextSnapshot(sourceFile, contents);
        }
    }
}
