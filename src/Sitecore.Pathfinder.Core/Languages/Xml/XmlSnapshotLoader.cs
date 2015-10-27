// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
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
            var text = sourceFile.ReadAsText(snapshotParseContext.Tokens);

            var xmlTextSnapshot = CompositionService.Resolve<XmlTextSnapshot>().With(snapshotParseContext, sourceFile, text, SchemaNamespace, SchemaFileName);

            return xmlTextSnapshot;
        }
    }
}
