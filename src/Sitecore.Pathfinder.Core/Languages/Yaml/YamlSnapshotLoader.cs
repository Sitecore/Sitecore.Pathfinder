// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    [Export(typeof(ISnapshotLoader))]
    public class YamlSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public YamlSnapshotLoader([NotNull] ICompositionService compositionService)
        {
            CompositionService = compositionService;
            Priority = 1000;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        public override bool CanLoad(ISourceFile sourceFile)
        {
            return string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".yaml", StringComparison.OrdinalIgnoreCase);
        }

        public override ISnapshot Load(ISourceFile sourceFile, SnapshotParseContext parseContext)
        {
            var contents = sourceFile.ReadAsText(parseContext.Tokens);

            var yamlTextSnapshot = CompositionService.Resolve<YamlTextSnapshot>().With(sourceFile, contents, parseContext);

            return yamlTextSnapshot;
        }
    }
}
