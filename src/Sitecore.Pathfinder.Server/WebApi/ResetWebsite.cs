// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.IO;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.IO.PathMappers;

namespace Sitecore.Pathfinder.WebApi
{
    public class ResetWebsite : IWebApi
    {
        [Diagnostics.NotNull, Import]
        public IPathMapperService PathMapper { get; private set; }

        public ActionResult Execute(IAppService app)
        {
            foreach (var mapper in PathMapper.WebsiteItemPathToProjectFileNames)
            {
                DeleteItems(mapper);
            }

            foreach (var mapper in PathMapper.WebsiteFileNameToProjectFileNames)
            {
                DeleteFiles(app.ProjectDirectory, mapper);
            }

            /*
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
            */

            return null;
        }

        protected virtual void DeleteFiles([Diagnostics.NotNull] string projectDirectory, [Diagnostics.NotNull] WebsiteFileNameToProjectFileNameMapper mapper)
        {
            DeleteFiles(mapper, FileUtil.MapPath("/"), FileUtil.MapPath(PathHelper.NormalizeItemPath(mapper.WebsiteDirectory)));
        }

        protected virtual void DeleteFiles([Diagnostics.NotNull] WebsiteFileNameToProjectFileNameMapper mapper, [Diagnostics.NotNull] string websiteDirectory, [Diagnostics.NotNull] string directoryOrFileName)
        {
            var websiteDirectoryOrFileName = '\\' + PathHelper.UnmapPath(websiteDirectory, directoryOrFileName);

            if (Directory.Exists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    Directory.Delete(directoryOrFileName, true);
                }
            }

            if (File.Exists(directoryOrFileName))
            {
                string projectFileName;
                if (mapper.TryGetProjectFileName(websiteDirectoryOrFileName, out projectFileName))
                {
                    File.Delete(directoryOrFileName);
                }
            }

            if (!Directory.Exists(directoryOrFileName))
            {
                return;
            }

            foreach (var fileName in Directory.GetFiles(directoryOrFileName, "*", SearchOption.TopDirectoryOnly))
            {
                DeleteFiles(mapper, websiteDirectory, fileName);
            }

            foreach (var directory in Directory.GetDirectories(directoryOrFileName, "*", SearchOption.TopDirectoryOnly))
            {
                DeleteFiles(mapper, websiteDirectory, directory);
            }
        }

        /*
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

        */

        protected virtual void DeleteItems([Diagnostics.NotNull] WebsiteItemPathToProjectFileNameMapper mapper)
        {
            var database = Factory.GetDatabase(mapper.DatabaseName);
            var item = database.GetItem(mapper.ItemPath);
            if (item == null)
            {
                return;
            }

            DeleteItems(mapper, item);
        }

        protected virtual void DeleteItems([Diagnostics.NotNull] WebsiteItemPathToProjectFileNameMapper mapper, [Diagnostics.NotNull] Item item)
        {
            string projectFileName;
            string format;
            if (mapper.TryGetProjectFileName(item.Paths.Path, item.TemplateName, out projectFileName, out format))
            {
                item.Recycle();
                return;
            }

            foreach (Item child in item.Children)
            {
                DeleteItems(mapper, child);
            }
        }
    }
}
