// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Tasks.SynchronizeWebsites
{
    [Export(nameof(SynchronizeWebsite), typeof(IWebsiteTask))]
    public class SynchronizeWebsite : WebsiteTaskBase
    {
        [ImportingConstructor]
        public SynchronizeWebsite([Diagnostics.NotNull, ItemNotNull, ImportMany(typeof(ISynchronizer))] IEnumerable<ISynchronizer> synchronizers) : base("server:synchronize-website")
        {
            Synchronizers = synchronizers;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<ISynchronizer> Synchronizers { get; }

        public override void Run(IWebsiteTaskContext context)
        {
            TempFolder.EnsureFolder();

            var fileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Resources.zip"));

            using (var zip = new ZipWriter(fileName))
            {
                foreach (var pair in context.Configuration.GetSubKeys("sync-website:files"))
                {
                    var configKey = "sync-website:files:" + pair.Key + ":";
                    var syncFileName = context.Configuration.GetString(configKey + "file");

                    foreach (var synchronizer in Synchronizers)
                    {
                        if (synchronizer.CanSynchronize(context.Configuration, syncFileName))
                        {
                            synchronizer.Synchronize(context.Configuration, zip, syncFileName, configKey);
                        }
                    }
                }
            }

            context.ActionResult = new FilePathResult(fileName, "application/zip");
        }
    }
}
