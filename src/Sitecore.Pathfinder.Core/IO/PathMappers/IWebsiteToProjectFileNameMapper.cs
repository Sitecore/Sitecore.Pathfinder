using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IWebsiteToProjectFileNameMapper
    {
        bool TryGetProjectFileName([NotNull] string websiteFileName, [NotNull] out string projectFileName);

        [NotNull]
        string WebsiteDirectory { get; }
    }
}