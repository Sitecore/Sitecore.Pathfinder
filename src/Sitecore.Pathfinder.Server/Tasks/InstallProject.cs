// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(InstallProject), typeof(IWebsiteTask))]
    public class InstallProject : WebsiteTaskBase
    {
        public InstallProject() : base("server:install-project")
        {
        }

        public override void Run(IWebsiteTaskContext context)
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var emitter = context.CompositionService.Resolve<IEmitterService>();
            emitter.Start();

            var response = output.ToString();
            if (!string.IsNullOrEmpty(response))
            {
                context.ActionResult = new ContentResult
                {
                    Content = response,
                    ContentType = "plain/text",
                    ContentEncoding = Encoding.UTF8
                };

                return;
            }

            context.ActionResult = new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
