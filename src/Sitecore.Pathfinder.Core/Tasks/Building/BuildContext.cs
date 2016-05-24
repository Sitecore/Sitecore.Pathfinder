// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Tasks.Building
{
    [Export(typeof(IBuildContext)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BuildContext : TaskContext, IBuildContext
    {
        [CanBeNull]
        private IProject _project;

        [ImportingConstructor]
        public BuildContext([NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [NotNull] IProjectService projectService) : base(configuration, traceService, fileSystem)
        {
            ProjectService = projectService;
        }

        public string DataFolderDirectory => Configuration.GetString(Constants.Configuration.DataFolderDirectory);

        public bool IsProjectLoaded => _project != null;

        public IList<IProjectItem> ModifiedProjectItems { get; } = new List<IProjectItem>();

        public IList<string> OutputFiles { get; } = new List<string>();

        public IProject Project => _project ?? (_project = ProjectService.LoadProjectFromConfiguration());

        public string ToolsDirectory => Configuration.GetToolsDirectory();

        public string WebsiteDirectory => Configuration.GetString(Constants.Configuration.WebsiteDirectory);

        [NotNull]
        protected IProjectService ProjectService { get; }
    }
}
