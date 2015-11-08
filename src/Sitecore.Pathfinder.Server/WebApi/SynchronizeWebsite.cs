// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Web.Mvc;
using Sitecore.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.WebApi.SynchronizeWebsites;
using Sitecore.Web;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.WebApi
{
    public class SynchronizeWebsite : IWebApi
    {
        [Diagnostics.NotNull]
        [ItemNotNull]
        [ImportMany(typeof(ISynchronizer))]
        public IEnumerable<ISynchronizer> Synchronizers { get; protected set; }

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

            var startup = new Startup();
            var compositionContainer = startup.RegisterCompositionService();

            compositionContainer.SatisfyImportsOnce(this);

            TempFolder.EnsureFolder();

            var syncFileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Sync.zip"));
            using (var zip = new ZipWriter(syncFileName))
            {
                foreach (var pair in configuration.GetSubKeys("sync-website:files"))
                {
                    var key = "sync-website:files:" + pair.Key + ":";
                    var fileName = configuration.Get(key + "file");

                    if (string.IsNullOrEmpty(fileName))
                    {
                        continue;
                    }

                    foreach (var synchronizer in Synchronizers)
                    {
                        if (synchronizer.CanSynchronize(configuration, fileName))
                        {
                            synchronizer.Synchronize(configuration, zip, fileName, key);
                        }
                    }
                }
            }

            return new FilePathResult(syncFileName, "application/zip");
        }
    }
}
