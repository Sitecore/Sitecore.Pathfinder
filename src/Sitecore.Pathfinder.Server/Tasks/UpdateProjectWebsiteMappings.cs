// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Net;
using System.Web.Mvc;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(nameof(UpdateProjectWebsiteMappings), typeof(IWebsiteTask))]
    public class UpdateProjectWebsiteMappings : WebsiteTaskBase
    {
        public UpdateProjectWebsiteMappings() : base("server:update-project-website-mappings")
        {
        }

        public override void Run(IWebsiteTaskContext context)
        {
            ProjectHost.Clear();

            context.ActionResult = new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
