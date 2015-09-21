// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Web;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Synchronizing
{
    public class SynchronizationManager
    {
        public SynchronizationManager()
        {
            var startup = new Startup();
            var compositionContainer = startup.RegisterCompositionService();

            compositionContainer.SatisfyImportsOnce(this);
        }

        [Diagnostics.NotNull]
        [Diagnostics.UsedImplicitly]
        [ImportMany(typeof(ISynchronizer))]
        public IEnumerable<ISynchronizer> ContentExporters { get; protected set; }

        [Diagnostics.NotNull]
        public string BuildZipFile()
        {
            var toolsDirectory = WebUtil.GetQueryString("t");
            if (string.IsNullOrEmpty(toolsDirectory))
            {
                return string.Empty;
            }

            // todo: move somewhere central
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());
            configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configuration.Set(Constants.Configuration.SystemConfigFileName, "scconfig.json");

            var configurationService = new ConfigurationService(configuration);
            configurationService.Load(LoadConfigurationOptions.None);

            TempFolder.EnsureFolder();

            var syncFileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Sync.zip"));
            using (var zip = new ZipWriter(syncFileName))
            {
                foreach (var pair in configuration.GetSubKeys("sync"))
                {
                    foreach (var exporter in ContentExporters)
                    {
                        var key = "sync:" + pair.Key + ":";
                        var fileName = configuration.Get(key + "file");

                        if (exporter.CanSynchronize(configuration, fileName))
                        {
                            exporter.Synchronize(configuration, zip, fileName, key);
                        }
                    }
                }
            }

            return syncFileName;
        }
    }
}
