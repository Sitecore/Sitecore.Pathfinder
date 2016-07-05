// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Pipelines.BeforeBuildPipelines;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CheckScconfig : BuildTaskBase, IPreRunTask
    {
        [ImportingConstructor]
        public CheckScconfig([NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelineService) : base("check-scconfig")
        {
            FileSystem = fileSystem;
            PipelineService = pipelineService;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull]
        protected IPipelineService PipelineService { get; }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.ProjectDirectory;
            if (!FileSystem.DirectoryExists(projectDirectory))
            {
                return;
            }

            var toolsDirectory = context.ToolsDirectory;
            if (string.Equals(projectDirectory, toolsDirectory, StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.WriteLine(Texts.Whoops__scc_exe_cannot_run_in_is_own_directory_, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var configFileName = PathHelper.Combine(projectDirectory, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
            if (!FileSystem.FileExists(configFileName))
            {
                return;
            }

            var projectUniqueId = context.Configuration.GetString(Constants.Configuration.ProjectUniqueId);
            if (string.Equals(projectUniqueId, "{project-unique-id}", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.WriteLine(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var hostName = context.Configuration.GetString(Constants.Configuration.HostName);
            if (string.Equals(hostName, "http://sitecore.default", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.WriteLine(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var websiteDirectory = context.Configuration.GetString(Constants.Configuration.WebsiteDirectory);
            if (string.Equals(websiteDirectory, "c:\\inetpub\\wwwroot\\Sitecore.Default\\Website", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.WriteLine(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            PipelineService.Resolve<BeforeBuildPipeline>().Execute(context);
        }
    }
}
