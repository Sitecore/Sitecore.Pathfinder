// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.ProjectTrees
{
    public class ProjectTree : IProjectTree
    {
        public ProjectTree([NotNull] string toolsDirectory, [NotNull] string projectDirectory, [NotNull] string websiteDirectory)
        {
            ToolsDirectory = toolsDirectory;
            WebsiteDirectory = websiteDirectory;
            ProjectDirectory = projectDirectory;
        }

        public IProject Project
        {
            get
            {
                var app = new Startup().WithToolsDirectory(ToolsDirectory).WithProjectDirectory(ProjectDirectory).Start();
                if (app == null)
                {
                    return null;
                }

                var project = app.CompositionService.Resolve<IProjectService>().LoadProjectFromConfiguration();
                return project;
            }
        }

        [NotNull]
        public string ProjectDirectory { get; }

        public IEnumerable<IProjectTreeItem> Roots
        {
            get
            {
                var root = new FileProjectTreeItem(new ProjectTreeUri(ProjectDirectory));
                yield return root;
            }
        }

        [NotNull]
        public string ToolsDirectory { get; }

        [NotNull]
        public string WebsiteDirectory { get; }
    }
}
