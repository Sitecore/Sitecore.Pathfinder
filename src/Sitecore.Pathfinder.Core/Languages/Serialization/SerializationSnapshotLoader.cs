// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Export(typeof(ISnapshotLoader))]
    public class SerializationSnapshotLoader : SnapshotLoaderBase
    {
        [ImportingConstructor]
        public SerializationSnapshotLoader([NotNull] ICompositionService compositionService)
        {
            CompositionService = compositionService;
            Priority = 1000;
        }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        public override bool CanLoad(ISourceFile sourceFile)
        {
            return string.Equals(Path.GetExtension(sourceFile.AbsoluteFileName), ".item", StringComparison.OrdinalIgnoreCase);
        }

        public override ISnapshot Load(ISourceFile sourceFile, IDictionary<string, string> tokens)
        {
            var textSnapshot = CompositionService.Resolve<TextSnapshot>().With(sourceFile);

            return textSnapshot;
        }
    }
}
