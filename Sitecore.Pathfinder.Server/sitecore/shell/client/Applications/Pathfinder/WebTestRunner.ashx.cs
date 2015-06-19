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
            var testRunnerName = context.Request["n"] ?? "NUnit";

            context.Response.ContentType = "text/plain";

            var output = new StringWriter();
            Console.SetOut(output);

            var unitTestRunner = new UnitTestRunner();
            unitTestRunner.RunTests(testRunnerName);

            output.Flush();
            context.Response.Write(output.ToString());
        }
    }
}
