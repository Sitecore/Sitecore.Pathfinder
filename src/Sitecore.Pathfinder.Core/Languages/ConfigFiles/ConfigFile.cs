using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.ConfigFiles
{
    public class ConfigFile : File
    {
        public ConfigFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath) : base(project, snapshot, filePath)
        {
        }
    }
}