// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Zip;

namespace Sitecore.Pathfinder.Synchronizing
{
    public interface ISynchronizationExporter
    {
        void Export([NotNull] ZipWriter zip);
    }
}
