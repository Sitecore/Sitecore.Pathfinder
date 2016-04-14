// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
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
        public BuildContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITraceService traceService, [NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelineService, [NotNull] IProjectService projectService) : base(compositionService, configuration, traceService, fileSystem, pipelineService)
        {
            ProjectService = projectService;
        }

        public string DataFolderDirectory => Configuration.GetString(Constants.Configuration.DataFolderDirectory);

        public IList<IProjectItem> ModifiedProjectItems { get; } = new List<IProjectItem>();

        public IList<string> OutputFiles { get; } = new List<string>();

        public IProject Project => _project ?? (_project = ProjectService.LoadProjectFromConfiguration());

        public string ProjectDirectory => Configuration.GetString(Constants.Configuration.ProjectDirectory);

        public string ToolsDirectory => Configuration.GetString(Constants.Configuration.ToolsDirectory);

        public string WebsiteDirectory => Configuration.GetString(Constants.Configuration.WebsiteDirectory);

        [NotNull]
        protected IProjectService ProjectService { get; }
    }
}
