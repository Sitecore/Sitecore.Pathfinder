// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    public interface IPathMapperService
    {
        void Clear();

        bool TryGetProjectFileName([NotNull] string itemPath, [NotNull] string templateName, [NotNull] out string projectFileName, [NotNull] out string format);

        bool TryGetProjectFileName([NotNull] string websiteFileName, [NotNull] out string projectFileName);

        bool TryGetWebsiteFileName([NotNull] string projectFileName, [NotNull] out string websiteFileName);

        bool TryGetWebsiteItemPath([NotNull] string projectFileName, [NotNull] out string databaseName, [NotNull] out string itemPath, out bool isImport, out bool uploadMedia);
    }
}
