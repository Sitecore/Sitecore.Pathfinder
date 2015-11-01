// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Web.Mvc;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Testing;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderWebTestRunnerController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var authenticateResult = this.AuthenticateUser();
            if (authenticateResult != null)
            {
                return authenticateResult;
            }

            var testRunnerName = Request["n"] ?? "NUnit";

            var output = new StringWriter();
            Console.SetOut(output);

            var unitTestRunner = new UnitTestRunner();
            unitTestRunner.RunTests(testRunnerName);

            output.Flush();

            return Content(output.ToString(), "text/plain");
        }
    }
}
