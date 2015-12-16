// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.WebApi.SynchronizeWebsites
{
    [Export(nameof(SynchronizeWebsite), typeof(IWebApi))]
    public class SynchronizeWebsite : IWebApi
    {
        [Diagnostics.NotNull, ImportMany(typeof(ISynchronizer)), ItemNotNull]
        public IEnumerable<ISynchronizer> Synchronizers { get; protected set; }

        public ActionResult Execute(IAppService app)
        {
            TempFolder.EnsureFolder();

            var fileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Resources.zip"));

            using (var zip = new ZipWriter(fileName))
            {
                foreach (var pair in app.Configuration.GetSubKeys("sync-website:files"))
                {
                    var configKey = "sync-website:files:" + pair.Key + ":";
                    var syncFileName = app.Configuration.GetString(configKey + "file");

                    foreach (var synchronizer in Synchronizers)
                    {
                        if (synchronizer.CanSynchronize(app.Configuration, syncFileName))
                        {
                            synchronizer.Synchronize(app.Configuration, zip, syncFileName, configKey);
                        }
                    }
                }
            }

            return new FilePathResult(fileName, "application/zip");
        }
    }
}
