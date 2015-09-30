// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Sitecore.Rocks.Server.Extensibility;
using Sitecore.Rocks.Server.Validations;
using Sitecore.Security.Authentication;
using Sitecore.Web;

namespace Sitecore.Pathfinder.Controllers
{
    public class PathfinderSitecoreCopController : Controller
    {
        [Diagnostics.NotNull]
        public ActionResult Index()
        {
            ExtensibilityLoader.Initialize();

            var userName = GetParameter("u");
            if (!string.IsNullOrEmpty(userName))
            {
                var password = GetParameter("p");
                AuthenticationManager.Login(userName, password);
            }

            int timeout;
            if (!int.TryParse(GetParameter("t", "600"), out timeout))
            {
                timeout = 600;
            }

            Server.ScriptTimeout = timeout; // 10 minutes

            var contextName = GetParameter("c", "Site");
            var databasesAndLanguages = GetParameter("d", "master^en");
            var inactiveValidations = GetParameter("v", string.Empty);
            var rootItemPath = GetParameter("i", string.Empty);
            var processSiteValidations = GetParameter("s", "True");

            var content = Process(contextName, databasesAndLanguages, inactiveValidations, rootItemPath, string.Compare(processSiteValidations, "True", StringComparison.InvariantCultureIgnoreCase) == 0);

            return Content(content, "text/xml");
        }

        [NotNull]
        private string GetParameter([NotNull] string name, [NotNull] string defaultValue = "")
        {
            var result = WebUtil.GetQueryString(name);
            if (string.IsNullOrEmpty(result))
            {
                result = WebUtil.GetFormValue(name);
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return defaultValue;
        }

        [NotNull]
        private string Process([NotNull] string contextName, [NotNull] string databasesAndLanguages, [NotNull] string inactiveValidations, [NotNull] string rootItemPath, bool processSiteValidations)
        {
            using (var writer = new StringWriter())
            {
                var output = new XmlTextWriter(writer)
                {
                    Formatting = Formatting.Indented
                };

                var options = new ValidationAnalyzerOptions
                {
                    ContextName = contextName,
                    InactiveValidations = inactiveValidations,
                    CustomValidations = string.Empty,
                    ProcessCustomValidations = false,
                    ProcessValidations = processSiteValidations,
                    Deep = true
                };

                options.ParseDatabaseAndLanguages(databasesAndLanguages);

                if (!string.IsNullOrEmpty(rootItemPath) && rootItemPath != "/" && rootItemPath != "/sitecore")
                {
                    var descriptor = options.DatabasesAndLanguages.FirstOrDefault();
                    if (descriptor == null)
                    {
                        return string.Empty;
                    }

                    var rootItem = descriptor.Database.GetItem(rootItemPath, descriptor.Language);
                    if (rootItem == null)
                    {
                        return string.Empty;
                    }

                    options.RootItem = rootItem;
                }

                var analyzer = new ValidationAnalyzer();
                analyzer.Process(output, options);

                output.Flush();
                writer.Flush();
                writer.Close();

                return writer.ToString();
            }
        }
    }
}
