// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Tasks.SynchronizeWebsites
{
    [InheritedExport]
    public interface ISynchronizer
    {
        bool CanSynchronize([NotNull] IConfiguration configuration, [NotNull] string fileName);

        void Synchronize([NotNull] IConfiguration configuration, [NotNull] ZipWriter zip, [NotNull] string fileName, [NotNull] string configKey);
    }
}
