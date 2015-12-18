using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.PathMappers
{
    public interface IProjectToWebsiteFileNameMapper
    {
        bool TryGetWebsiteFileName([NotNull] string projectFileName, [NotNull] out string websiteFileName);
    }
}