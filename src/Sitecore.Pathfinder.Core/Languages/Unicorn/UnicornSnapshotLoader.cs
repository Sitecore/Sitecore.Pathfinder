// © 2015-2017 by Jakob Christensen. All rights reserved.

using System;
using System.Composition;
using System.IO;
using System.Text;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Unicorn
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class UnicornSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public UnicornSnapshotLoader([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanLoad(ISourceFile sourceFile) => string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".yml", StringComparison.OrdinalIgnoreCase);

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            var lines = sourceFile.ReadAsLines(snapshotParseContext.Tokens);

            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.StartsWith("---"))
                {
                    sb.AppendLine("- Item:");
                }
                else
                {
                    sb.AppendLine("  " + line);
                }
            }

            return Factory.UnicornTextSnapshot(sourceFile, sb.ToString());
        }
    }
}
