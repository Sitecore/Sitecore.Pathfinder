// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectOptions
    {
        [NotNull]
        public static readonly ProjectOptions Empty = new ProjectOptions(string.Empty, string.Empty);

        public ProjectOptions([NotNull] string projectDirectory, [NotNull] string databaseName)
        {
            ProjectDirectory = projectDirectory;
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public string ProjectDirectory { get; }

        [NotNull, ItemNotNull]
        public ICollection<string> StandardTemplateFields { get; } = new List<string>();

        [NotNull]
        public IDictionary<string, string> Tokens { get; } = new Dictionary<string, string>();
    }
}
