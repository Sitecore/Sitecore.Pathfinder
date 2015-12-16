// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Configuration;
using Sitecore.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects
{
    public static class ProjectHost
    {
        private static bool _isLoaded;

        [Diagnostics.NotNull, ItemNotNull]
        private static readonly ICollection<IProjectContext> ProjectContexts = new List<IProjectContext>();

        [Diagnostics.NotNull, ItemNotNull]
        public static IEnumerable<IProjectContext> Projects
        {
            get
            {
                if (!_isLoaded)
                {
                    LoadProjects();
                }

                return ProjectContexts;
            }
        }

        public static void Clear()
        {
            ProjectContexts.Clear();
            _isLoaded = false;
        }

        public static void Initialize()
        {
            _isLoaded = false;
        }

        public static void LoadProjects()
        {
            _isLoaded = true;

            ProjectContexts.Clear();

            var dataFolder = FileUtil.MapPath(Settings.DataFolder);
            var pathfinderFolder = Path.Combine(dataFolder, "Pathfinder");

            var fileName = Path.Combine(pathfinderFolder, "projects." + Environment.MachineName + ".xml");
            if (!FileUtil.FileExists(fileName))
            {
                return;
            }

            var xml = FileUtil.ReadFromFile(fileName);
            var root = xml.ToXElement();
            if (root == null)
            {
                return;
            }

            foreach (var element in root.Elements())
            {
                var toolsDirectory = element.GetAttributeValue("toolsdirectory");
                if (!Directory.Exists(toolsDirectory))
                {
                    continue;
                }

                var projectDirectory = element.GetAttributeValue("projectdirectory");
                if (!Directory.Exists(projectDirectory))
                {
                    continue;
                }

                var app = new Startup().WithToolsDirectory(toolsDirectory).WithProjectDirectory(projectDirectory).Start();
                if (app == null)
                {
                    throw new ConfigurationException("Failed to load configuration");
                }

                var projectService = app.CompositionService.Resolve<IProjectService>();
                var projectOptions = projectService.GetProjectOptions();
                var projectTree = projectService.GetProjectTree(projectOptions).With(toolsDirectory, projectDirectory);

                var projectContext = new ProjectContext(app.Configuration, app.CompositionService, projectTree);

                ProjectContexts.Add(projectContext);
            }
        }
    }
}
