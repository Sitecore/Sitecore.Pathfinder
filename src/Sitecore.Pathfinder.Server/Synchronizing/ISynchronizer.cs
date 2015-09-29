// © 2015 Sitecore Corporation A/S. All rights reserved.

using Microsoft.Framework.ConfigurationModel;
using Sitecore.Zip;

namespace Sitecore.Pathfinder.Synchronizing
{
    public interface ISynchronizer
    {
        bool CanSynchronize([Diagnostics.NotNull] IConfiguration configuration, [Diagnostics.NotNull] string fileName);

        void Synchronize([Diagnostics.NotNull] IConfiguration configuration, [NotNull] ZipWriter zip, [Diagnostics.NotNull] string fileName, [Diagnostics.NotNull] string configKey);
    }
}
