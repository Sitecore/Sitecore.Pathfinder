// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Web.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(IWebsiteTaskContext))]
    public class WebsiteTaskContext : TaskContext, IWebsiteTaskContext
    {
        [ImportingConstructor]
        public WebsiteTaskContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelineService) : base(compositionService, configuration, traceService, fileSystem, pipelineService)
        {
        }

        public ActionResult ActionResult { get; set; }

        public IAppService App { get; private set; }

        public IWebsiteTaskContext With(IAppService app)
        {
            App = app;
            return this;
        }
    }
}
