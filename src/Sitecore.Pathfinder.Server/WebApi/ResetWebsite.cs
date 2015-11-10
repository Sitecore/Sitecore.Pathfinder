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
                if (pair.Key == "website")
                {
                    DeleteFiles(configuration, "website", FileUtil.MapPath("/"));
                }
                else if (pair.Key == "data")
                {
                    DeleteFiles(configuration, "data", FileUtil.MapPath(Settings.DataFolder));
                }
                else
                {
                    DeleteItems(configuration, pair.Key);
                }
            }

            return null;
        }

        protected virtual void DeleteFiles([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] string area, [Diagnostics.NotNull] string baseDirectory)
        {
            foreach (var pair in configuration.GetSubKeys("reset-website:" + area))
            {
                var key = "reset-website:" + area + ":" + pair.Key;

                var path = configuration.GetString(key + ":path");
                var include = configuration.GetString(key + ":include");
                var exclude = configuration.GetString(key + ":exclude");

                path = PathHelper.NormalizeFilePath(Path.Combine(baseDirectory, PathHelper.NormalizeFilePath(path).TrimStart('\\'))).TrimEnd('\\');

                if (File.Exists(path))
                {
                    FileUtil.Delete(path);
                    continue;
                }

                if (Directory.Exists(path) && string.IsNullOrEmpty(include) && string.IsNullOrEmpty(exclude))
                {
                    FileUtil.DeleteDirectory(path, true);
                    continue;
                }

                if (!Directory.Exists(path))
                {
                    continue;
                }

                include = path + "\\" + PathHelper.NormalizeFilePath(include).TrimStart('\\');
                exclude = path + "\\" + PathHelper.NormalizeFilePath(exclude).TrimStart('\\');

                var matcher = new PathMatcher(include, exclude);

                foreach (var fileName in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
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
                var queryText = configuration.GetString("reset-website:" + databaseName + ":" + pair.Key + ":query");

                foreach (var item in database.Query(queryText))
                {
                    item.Recycle();
                }
            }
        }
    }
}
