// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Net;
using System.Web.Mvc;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.WebApi
{
    [Export(nameof(UpdateProjectWebsiteMappings), typeof(IWebApi))]
    public class UpdateProjectWebsiteMappings : IWebApi
    {
        public ActionResult Execute(IAppService app)
        {
            ProjectHost.Clear();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
