// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Newtonsoft.Json;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class NewSolution : NewProjectTaskBase
    {
        [ImportingConstructor]
        public NewSolution([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] IFileSystemService fileSystem) : base(console, fileSystem, "new-solution")
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public override void Run(IBuildContext context)
        {
            CreateProject(context, NewProjectOptions.CopyConfig | NewProjectOptions.CopyCmd);

            var solutionDirectory = Configuration.GetProjectDirectory();

            var projectDirectories = new List<string>();
            GetProjectDirectories(solutionDirectory, projectDirectories);

            var solutionFileName = Path.Combine(solutionDirectory, "scconfig.solution.json");

            using (var writer = new StreamWriter(solutionFileName))
            {
                var output = new JsonTextWriter(writer);
                output.Formatting = Formatting.Indented;

                output.WriteStartObject();
                output.WriteStartObject("projects");

                foreach (var projectDirectory in projectDirectories)
                {
                    var relativePath = PathHelper.NormalizeItemPath(PathHelper.UnmapPath(solutionDirectory, projectDirectory));
                    var projectName = relativePath.Replace("/", ".");

                    output.WritePropertyString(projectName, relativePath);
                }

                output.WriteEndObject();
                output.WriteEndObject();
            }
        }

        protected virtual void GetProjectDirectories([NotNull] string solutionDirectory, [NotNull, ItemNotNull] List<string> projectDirectories)
        {
            foreach (var fileName in FileSystem.GetFiles(solutionDirectory, "scconfig.json", SearchOption.AllDirectories))
            {
                GetProjectDirectory(solutionDirectory, projectDirectories, fileName);
            }

            foreach (var fileName in FileSystem.GetFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories))
            {
                GetProjectDirectory(solutionDirectory, projectDirectories, fileName);
            }
        }

        protected virtual void GetProjectDirectory([NotNull] string solutionDirectory, [NotNull, ItemNotNull] List<string> projectDirectories, [NotNull] string fileName)
        {
            var projectDirectory = PathHelper.NormalizeItemPath(Path.GetDirectoryName(fileName) ?? string.Empty);
            if (string.IsNullOrEmpty(projectDirectory))
            {
                return;
            }

            if (projectDirectory.IndexOf("/sitecore.project/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            if (projectDirectory.IndexOf("/packages/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            if (projectDirectory.IndexOf("/node_modules/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            var projectName = Path.GetFileName(projectDirectory);

            if (projectName.IndexOf("tests", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            if (string.Equals(projectName, "code", StringComparison.OrdinalIgnoreCase))
            {
                projectDirectory = PathHelper.NormalizeItemPath(Path.GetDirectoryName(projectDirectory) ?? string.Empty);
                if (string.IsNullOrEmpty(projectDirectory))
                {
                    return;
                }
            }

            if (string.Equals(projectDirectory, PathHelper.NormalizeItemPath(solutionDirectory), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!projectDirectories.Contains(projectDirectory))
            {
                projectDirectories.Add(projectDirectory);
            }
        }
    }
}
