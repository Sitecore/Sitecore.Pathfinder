// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks.Building
{
    public class BuildContext : TaskContext, IBuildContext
    {
        [NotNull]
        private Func<IProject> _loadProject;

        [CanBeNull]
        private IProject _project;

        [FactoryConstructor(typeof(IBuildContext))]
        public BuildContext([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] ITraceService traceService) : base(configuration, console, traceService)
        {
        }

        public string DataFolderDirectory => Configuration.GetString(Constants.Configuration.DataFolderDirectory);

        public bool IsProjectLoaded => _project != null;

        public ICollection<OutputFile> OutputFiles { get; } = new List<OutputFile>();

        public string ProjectDirectory => Configuration.GetProjectDirectory();

        public string ToolsDirectory => Configuration.GetToolsDirectory();

        public IProject LoadProject()
        {
            if (_project != null)
            {
                return _project;
            }

            var project = _loadProject();

            _project = project;
            return project;
        }

        public IBuildContext With(Func<IProject> loadProject)
        {
            _loadProject = loadProject;
            return this;
        }
    }
}
