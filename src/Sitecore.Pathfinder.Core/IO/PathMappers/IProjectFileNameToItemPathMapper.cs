using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IProjectFileNameToItemPathMapper
    {
        bool TryGetWebsiteItemPath([NotNull] string projectFileName, [NotNull] out string databaseName, [NotNull] out string itemPath, out bool isImport, out bool uploadMedia);
    }
}