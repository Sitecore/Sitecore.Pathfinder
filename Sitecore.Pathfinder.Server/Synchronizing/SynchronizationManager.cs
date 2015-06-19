// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.IO;
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
        [ImportMany(typeof(ISynchronizationExporter))]
        public IEnumerable<ISynchronizationExporter> Exporters { get; protected set; }

        [Diagnostics.NotNull]
        public string BuildResourceFile()
        {
            TempFolder.EnsureFolder();

            var fileName = FileUtil.MapPath(TempFolder.GetFilename("Pathfinder.Resources.zip"));
            using (var zip = new ZipWriter(fileName))
            {
                foreach (var exporter in Exporters)
                {
                    exporter.Export(zip);
                }
            }

            return fileName;
        }
    }
}
