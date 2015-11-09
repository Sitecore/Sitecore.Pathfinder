// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Building.Initializing.BeforeBuilds
{
    public class BeforeBuild : TaskBase
    {
        [ImportingConstructor]
        public BeforeBuild([NotNull] IPipelineService pipelineService) : base("before-build")
        {
            PipelineService = pipelineService;
        }

        [NotNull]
        protected IPipelineService PipelineService { get; }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.ProjectDirectory;
            if (!context.FileSystem.DirectoryExists(projectDirectory))
            {
                return;
            }

            var toolsDirectory = context.Configuration.GetString(Constants.Configuration.ToolsDirectory);
            if (string.Equals(projectDirectory, toolsDirectory, StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.Writeline(Texts.Whoops__scc_exe_cannot_run_in_is_own_directory_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var configFileName = PathHelper.Combine(projectDirectory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
            if (!context.FileSystem.FileExists(configFileName))
            {
                return;
            }

            var projectUniqueId = context.Configuration.Get(Constants.Configuration.ProjectUniqueId);
            if (string.Equals(projectUniqueId, "{project-unique-id}", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.Writeline(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var hostName = context.Configuration.Get(Constants.Configuration.HostName);
            if (string.Equals(hostName, "http://sitecore.default", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.Writeline(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var websiteDirectory = context.Configuration.Get(Constants.Configuration.WebsiteDirectory);
            if (string.Equals(websiteDirectory, "c:\\inetpub\\wwwroot\\Sitecore.Default\\Website", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.Writeline(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var sourceDirectory = Path.Combine(context.Configuration.Get(Constants.Configuration.ToolsDirectory), "files\\website");
            var coreServerAssemblyFileName = Path.Combine(websiteDirectory, "bin\\Sitecore.Pathfinder.Core.dll");
            if (!context.FileSystem.FileExists(coreServerAssemblyFileName))
            {
                context.FileSystem.XCopy(sourceDirectory, websiteDirectory);
                context.Trace.Writeline(Texts.Just_so_you_know__I_have_copied_the__Sitecore_Pathfinder_Server_dll__and__NuGet_Core_dll__assemblies_to_the___bin__directory_in_the_website_and_a_number_of___aspx__files_to_the___sitecore_shell_client_Applications_Pathfinder__directory);
            }
            else
            {
                UpdateFiles(context, sourceDirectory, websiteDirectory);
            }

            UpdateConfigFile(context, toolsDirectory, websiteDirectory);

            PipelineService.Resolve<BeforeBuildPipeline>().Execute(context);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
        }

        protected virtual void UpdateConfigFile([NotNull] IBuildContext context, [NotNull] string toolsDirectory, [NotNull] string websiteDirectory)
        {
            var projectConfigFileName = context.Configuration.GetString(Constants.Configuration.ProjectConfigFileName);

            var sourceFileName = Path.Combine(toolsDirectory, projectConfigFileName);
            var targetFileName = Path.Combine(websiteDirectory, "bin\\" + projectConfigFileName);

            var sourceFileInfo = new FileInfo(sourceFileName);
            var targetFileInfo = new FileInfo(targetFileName);

            if (!targetFileInfo.Exists || sourceFileInfo.Length != targetFileInfo.Length)
            {
                context.FileSystem.Copy(sourceFileName, targetFileName);
            }
        }

        protected virtual void UpdateFiles([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string websiteDirectory)
        {
            var writeMessage = false;

            foreach (var sourceFileName in context.FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories))
            {
                var targetFileName = Path.Combine(websiteDirectory, sourceFileName.Mid(sourceDirectory.Length + 1));
                if (!context.FileSystem.FileExists(targetFileName))
                {
                    context.FileSystem.Copy(sourceFileName, targetFileName);
                    writeMessage = true;
                    continue;
                }

                if (string.Equals(Path.GetExtension(sourceFileName), ".dll", StringComparison.OrdinalIgnoreCase))
                {
                    var sourceVersion = new Version(FileVersionInfo.GetVersionInfo(sourceFileName).FileVersion);
                    var targetVersion = new Version(FileVersionInfo.GetVersionInfo(targetFileName).FileVersion);
                    if (targetVersion < sourceVersion)
                    {
                        context.FileSystem.Copy(sourceFileName, targetFileName);
                        writeMessage = true;
                    }

                    continue;
                }

                var sourceFileInfo = new FileInfo(sourceFileName);
                var targetFileInfo = new FileInfo(targetFileName);
                if (sourceFileInfo.Length != targetFileInfo.Length)
                {
                    context.FileSystem.Copy(sourceFileName, targetFileName);
                    writeMessage = true;
                }
            }

            if (writeMessage)
            {
                context.Trace.Writeline(Texts.Just_so_you_know__I_have_updated_the__Sitecore_Pathfinder_Server_dll__and__NuGet_Core_dll__assemblies_in_the___bin__directory_in_the_website_and_a_number_of___aspx__files_in_the___sitecore_shell_client_Applications_Pathfinder__directory_to_the_latest_version);
            }
        }
    }
}
