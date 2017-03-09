// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;

namespace Sitecore.Pathfinder.Emitting.ItemsAndFilesEmitting
{
    [Export(typeof(FilesProjectEmitter))]
    public class FilesProjectEmitter : ProjectEmitter
    {
        [ImportingConstructor]
        public FilesProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters) : base(configuration, compositionService, traceService, emitters)
        {
        }
    }
}
