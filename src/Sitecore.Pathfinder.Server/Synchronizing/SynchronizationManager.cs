// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.IO;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
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
        [ItemNotNull]
        [ImportMany(typeof(ISynchronizer))]
        public IEnumerable<ISynchronizer> Synchronizers { get; protected set; }

        [Diagnostics.NotNull]
        public virtual string BuildSyncFile()
        {
            var toolsDirectory = WebUtil.GetQueryString("t");
            if (string.IsNullOrEmpty(toolsDirectory))
            {
                return string.Empty;
            }

            var configuration = LoadProjectConfiguration(toolsDirectory);

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

            return syncFileName;
        }

        [Diagnostics.NotNull]
        protected virtual IConfiguration LoadProjectConfiguration([Diagnostics.NotNull] string toolsDirectory)
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());
            configuration.Set(Constants.Configuration.ToolsDirectory, toolsDirectory);
            configuration.Set(Constants.Configuration.SystemConfigFileName, "scconfig.json");

            var configurationService = new ConfigurationService(configuration);
            configurationService.Load(LoadConfigurationOptions.None);
            return configuration;
        }
    }
}
