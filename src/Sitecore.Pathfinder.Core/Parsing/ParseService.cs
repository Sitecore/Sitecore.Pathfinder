// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing
{
    [Export(typeof(IParseService))]
    public class ParseService : IParseService
    {
        [ImportingConstructor]
        public ParseService([NotNull] ICompositionService compositionService, [NotNull] ISnapshotService snapshotService)
        {
            CompositionService = compositionService;
            SnapshotService = snapshotService;
        }

        [NotNull]
        [ImportMany]
        [ItemNotNull]
        public IEnumerable<IParser> Parsers { get; private set; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        protected ISnapshotService SnapshotService { get; }

        public virtual void Parse(IProject project, ISourceFile sourceFile)
        {
            var snapshot = SnapshotService.LoadSnapshot(project, sourceFile);

            var parseContext = CompositionService.Resolve<IParseContext>().With(project, snapshot);

            foreach (var parser in Parsers.OrderBy(c => c.Sortorder))
            {
                try
                {
                    if (parser.CanParse(parseContext))
                    {
                        parser.Parse(parseContext);
                    }
                }
                catch (Exception ex)
                {
                    parseContext.Trace.TraceError(ex.Message, sourceFile.FileName, TextSpan.Empty);
                }
            }
        }
    }
}
