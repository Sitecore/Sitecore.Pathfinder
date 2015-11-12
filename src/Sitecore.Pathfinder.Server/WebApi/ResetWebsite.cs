// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Web.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.WebApi
{
    public class ResetWebsite : IWebApi
    {
        public ActionResult Execute(IAppService app)
        {
            foreach (var pair in app.Configuration.GetSubKeys("reset-website"))
            {
                if (pair.Key == "website")
                {
                    DeleteFiles(app.Configuration, "website", FileUtil.MapPath("/"));
                }
                else if (pair.Key == "data")
                {
                    DeleteFiles(app.Configuration, "data", FileUtil.MapPath(Settings.DataFolder));
                }
                else
                {
                    DeleteItems(app.Configuration, pair.Key);
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

        protected virtual void DeleteItems([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] string databaseName)
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
