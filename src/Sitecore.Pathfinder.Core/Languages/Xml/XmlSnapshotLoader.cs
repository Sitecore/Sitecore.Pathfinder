// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    [Export(typeof(ISnapshotLoader))]
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

        public override ISnapshot Load(ISourceFile sourceFile, IDictionary<string, string> tokens)
        {
            var text = sourceFile.ReadAsText(tokens);

            var xmlTextSnapshot = CompositionService.Resolve<XmlTextSnapshot>().With(sourceFile, text, tokens, SchemaNamespace, SchemaFileName);

            return xmlTextSnapshot;
        }
    }
}
