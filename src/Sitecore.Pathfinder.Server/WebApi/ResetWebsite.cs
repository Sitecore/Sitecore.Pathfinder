// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Web;

namespace Sitecore.Pathfinder.WebApi
{
    public class ResetWebsite : IWebApi
    {
        public ActionResult Execute()
        {
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

            foreach (var pair in configuration.GetSubKeys("reset-website"))
            {
                if (pair.Key == "files")
                {
                    DeleteFiles(configuration);
                }
                else
                {
                    DeleteItems(configuration, pair.Key);
                }
            }

            return null;
        }

        protected virtual void DeleteFiles([Diagnostics.NotNull] IConfigurationSourceRoot configuration)
        {
            foreach (var pair in configuration.GetSubKeys("reset-website:files"))
            {
                var directory = FileUtil.MapPath(pair.Key);
                var value = configuration.Get("reset-website:files:" + pair.Key);
                var include = PathHelper.NormalizeFilePath(directory).TrimEnd('\\') + "\\" + PathHelper.NormalizeFilePath(value).TrimStart('\\');

                var matcher = new PathMatcher(include, string.Empty);

                foreach (var fileName in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    if (!matcher.IsMatch(fileName))
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        protected virtual void DeleteItems([Diagnostics.NotNull] IConfigurationSourceRoot configuration, [Diagnostics.NotNull] string databaseName)
        {
            var database = Factory.GetDatabase(databaseName);

            foreach (var pair in configuration.GetSubKeys("reset-website:" + databaseName))
            {
                foreach (var item in database.Query(pair.Key))
                {
                    item.Recycle();
                }
            }
        }
    }
}
