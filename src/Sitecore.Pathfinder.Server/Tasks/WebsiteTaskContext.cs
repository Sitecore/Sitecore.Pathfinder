// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Web.Mvc;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(IWebsiteTaskContext))]
    public class WebsiteTaskContext : TaskContext, IWebsiteTaskContext
    {
        [ImportingConstructor]
        public WebsiteTaskContext([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] IConsoleService console, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem) : base(configuration, compositionService, console, traceService, fileSystem)
        {
        }

        public ActionResult ActionResult { get; set; }

        public IHostService Host { get; private set; }

        public IWebsiteTaskContext With(IHostService host)
        {
            Host = host;
            return this;
        }
    }
}
