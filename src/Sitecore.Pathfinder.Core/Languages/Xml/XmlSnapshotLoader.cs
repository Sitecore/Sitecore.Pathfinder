using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class XmlSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public XmlSnapshotLoader([NotNull] IFactory factory) : base(1000)
        {
            Factory = factory;
        }

        // todo: handle schemas
        [NotNull]
        public string SchemaFileName { get; private set; } = string.Empty;

        [NotNull]
        public string SchemaNamespace { get; private set; } = string.Empty;

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanLoad(ISourceFile sourceFile) => string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".xml", StringComparison.OrdinalIgnoreCase);

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            var contents = sourceFile.ReadAsText(snapshotParseContext.Tokens);

            return Factory.XmlTextSnapshot(sourceFile, contents, SchemaNamespace, SchemaFileName);
        }
    }
}
