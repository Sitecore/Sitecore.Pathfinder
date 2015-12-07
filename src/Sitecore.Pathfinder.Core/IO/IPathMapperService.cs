// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    public interface IPathMapperService
    {
        void Clear();

        bool TryGetProjectFileName([NotNull] string itemPath, [NotNull] string templateName, [CanBeNull] out string projectFileName, [CanBeNull] out string format);

        bool TryGetProjectFileName([NotNull] string websiteFileName, [CanBeNull] out string projectFileName);

        bool TryGetWebsiteFileName([NotNull] string projectFileName, [CanBeNull] out string websiteFileName);

        bool TryGetWebsiteItemPath([NotNull] string projectFileName, [CanBeNull] out string itemPath);
    }
}
