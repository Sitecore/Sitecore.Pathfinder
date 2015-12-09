// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Net;
using System.Web.Mvc;
using Sitecore.Pathfinder.Serializing;

namespace Sitecore.Pathfinder.WebApi
{
    [Export(nameof(UpdateProjectWebsiteMappings), typeof(IWebApi))]
    public class UpdateProjectWebsiteMappings : IWebApi
    {
        public ActionResult Execute(IAppService app)
        {
            SerializingDataProviderService.Reload();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
