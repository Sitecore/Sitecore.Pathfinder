// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Emitting.FileAndSqlEmitting
{
    [Export(typeof(FileAndSqlProjectEmitter))]
    public class FileAndSqlProjectEmitter : ProjectEmitter
    {
        public FileAndSqlProjectEmitter([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService traceService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IEmitter> emitters) : base(configuration, compositionService, traceService, emitters)
        {
        }
    }
}
