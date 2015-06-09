// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Zip;

namespace Sitecore.Pathfinder.Resources
{
    public interface IResourceExporter
    {
        void Export([NotNull] ZipWriter zip);
    }
}
