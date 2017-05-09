// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IWebsiteToProjectFileNameMapper
    {
        [NotNull]
        string WebsiteDirectory { get; }

        bool TryGetProjectFileName([NotNull] string websiteFileName, [NotNull] out string projectFileName);
    }
}
