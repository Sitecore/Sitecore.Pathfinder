// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Web;

namespace Sitecore.Pathfinder.WebApi
{
    public class InstallProject : IWebApi
    {
        public ActionResult Execute()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var toolsDirectory = WebUtil.GetQueryString("td");
            if (string.IsNullOrEmpty(toolsDirectory))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Project Directory not specified");
            }

            var projectDirectory = WebUtil.GetQueryString("pd");
            if (string.IsNullOrEmpty(projectDirectory))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Tools Directory not specified");
            }

            var configuration = ConfigurationStartup.RegisterConfiguration(toolsDirectory, projectDirectory, ConfigurationOptions.Noninteractive);
            if (configuration == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Configuration failed");
            }

            var emitService = new EmitService(toolsDirectory, projectDirectory);
            emitService.Start();

            var response = output.ToString();
            if (!string.IsNullOrEmpty(response))
            {
                return new ContentResult()
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
