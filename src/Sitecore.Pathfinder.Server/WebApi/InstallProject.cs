// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.WebApi
{
    public class InstallProject : IWebApi
    {
        public ActionResult Execute(IAppService app)
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var emitter = app.CompositionService.Resolve<Emitter>();
            emitter.Start();

            var response = output.ToString();
            if (!string.IsNullOrEmpty(response))
            {
                return new ContentResult
                {
                    Content = response,
                    ContentType = "plain/text",
                    ContentEncoding = Encoding.UTF8
                };
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
