// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class YamlSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public YamlSnapshotLoader([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanLoad(ISourceFile sourceFile) => string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".yaml", StringComparison.OrdinalIgnoreCase);

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            var contents = sourceFile.ReadAsText(snapshotParseContext.Tokens);

            return Factory.YamlTextSnapshot(sourceFile, contents);
        }
    }
}
