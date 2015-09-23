// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Zip;

namespace Sitecore.Pathfinder.Synchronizing
{
    public interface ISynchronizer
    {
        bool CanSynchronize([Diagnostics.NotNull] Microsoft.Framework.ConfigurationModel.Configuration configuration, [Diagnostics.NotNull] string fileName);

        void Synchronize([Diagnostics.NotNull] Microsoft.Framework.ConfigurationModel.Configuration configuration, [NotNull] ZipWriter zip, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string configKey);
    }
}
