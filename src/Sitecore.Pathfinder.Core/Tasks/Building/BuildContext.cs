// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks.Building
{
    [Export(typeof(IBuildContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BuildContext : TaskContext, IBuildContext
    {
        [NotNull]
        private Func<IProject> _loadProject;

        [CanBeNull]
        private IProject _project;

        [ImportingConstructor]
        public BuildContext([NotNull] IConfiguration configuration, [NotNull] IConsoleService console, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem) : base(configuration, console, traceService, fileSystem)
        {
        }

        public string DataFolderDirectory => Configuration.GetString(Constants.Configuration.DataFolderDirectory);

        public bool IsProjectLoaded => Project != Projects.Project.Empty;

        public ICollection<IProjectItem> ModifiedProjectItems { get; } = new List<IProjectItem>();

        public ICollection<OutputFile> OutputFiles { get; } = new List<OutputFile>();

        public IProject Project => _project ?? (_project = _loadProject());

        public string ProjectDirectory => Configuration.GetProjectDirectory();

        public string ToolsDirectory => Configuration.GetToolsDirectory();

        public string WebsiteDirectory => Configuration.GetWebsiteDirectory();

        public IBuildContext With(Func<IProject> loadProject)
        {
            _loadProject = loadProject;
            return this;
        }
    }
}
