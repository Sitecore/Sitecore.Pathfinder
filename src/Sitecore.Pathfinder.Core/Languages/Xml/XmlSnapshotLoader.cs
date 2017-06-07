// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader)), Shared]
    public class XmlSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public XmlSnapshotLoader([NotNull] ICompositionService compositionService)
        {
            CompositionService = compositionService;
            Priority = 1000;
        }

        [NotNull]
        public string SchemaFileName { get; protected set; } = string.Empty;

        [NotNull]
        public string SchemaNamespace { get; protected set; } = string.Empty;

        [NotNull]
        protected ICompositionService CompositionService { get; }

        public override bool CanLoad(ISourceFile sourceFile)
        {
            return string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".xml", StringComparison.OrdinalIgnoreCase);
        }

        public override ISnapshot Load(SnapshotParseContext snapshotParseContext, ISourceFile sourceFile)
        {
            var contents = sourceFile.ReadAsText(snapshotParseContext.Tokens);

            return CompositionService.Resolve<XmlTextSnapshot>().With(sourceFile, contents, SchemaNamespace, SchemaFileName);
        }
    }
}
