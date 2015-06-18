// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Web;
using Sitecore.Pathfinder.Testing;

namespace Sitecore.Pathfinder.Shell.Client.Applications.Pathfinder
{
    public class WebTestRunner : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var output = new StringWriter();
            Console.SetOut(output);
            // Console.SetOut(new StreamWriter(context.Response.OutputStream));

            var unitTestRunner = new UnitTestRunner();

            unitTestRunner.RunTests();

            context.Response.Write(output.ToString());
        }
    }
}
