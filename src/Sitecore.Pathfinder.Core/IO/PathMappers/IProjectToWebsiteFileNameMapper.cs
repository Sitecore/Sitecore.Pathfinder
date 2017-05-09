// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IProjectToWebsiteFileNameMapper
    {
        bool TryGetWebsiteFileName([NotNull] string projectFileName, [NotNull] out string websiteFileName);
    }
}
