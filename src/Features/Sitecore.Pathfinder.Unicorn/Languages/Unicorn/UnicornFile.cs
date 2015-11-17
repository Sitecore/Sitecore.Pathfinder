// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Files;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Unicorn.Languages.Unicorn
{
    public class UnicornFile : File
    {
        public UnicornFile([NotNull] IProject project, [NotNull] ISnapshot snapshot, [NotNull] string filePath, [NotNull] string databaseName) : base(project, snapshot, filePath)
        {
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }
    }
}
