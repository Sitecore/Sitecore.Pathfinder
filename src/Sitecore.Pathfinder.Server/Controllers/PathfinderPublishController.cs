using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Publishing;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderPublishController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var databaseName = WebUtil.GetQueryString("db", "master");
            var mode = WebUtil.GetQueryString("m", "i");

            var database = Factory.GetDatabase(databaseName);

            var publishingTargets = PublishManager.GetPublishingTargets(database);

            var targetDatabases = publishingTargets.Select(target => Factory.GetDatabase(target["Target database"])).ToArray();
            if (!targetDatabases.Any())
            {
                return new EmptyResult();
            }

            var languages = LanguageManager.GetLanguages(database).ToArray();

            switch (mode)
            {
                case "r":
                    PublishManager.Republish(database, targetDatabases, languages);
                    break;

                case "i":
                    PublishManager.PublishIncremental(database, targetDatabases, languages);
                    break;

                case "s":
                    PublishManager.PublishSmart(database, targetDatabases, languages);
                    break;

                case "b":
                    PublishManager.RebuildDatabase(database, targetDatabases);
                    break;
            }

            return Content(output.ToString(), "text/plain");
        }
    }
}