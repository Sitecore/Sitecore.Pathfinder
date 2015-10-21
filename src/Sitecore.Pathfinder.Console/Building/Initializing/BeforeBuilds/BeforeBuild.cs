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
    [Export(typeof(ITask))]
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
            var projectDirectory = context.SolutionDirectory;
            if (!context.FileSystem.DirectoryExists(projectDirectory))
            {
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

            var wwwroot = context.Configuration.Get(Constants.Configuration.Wwwroot);
            if (string.Equals(wwwroot, "c:\\inetpub\\wwwroot\\Sitecore.Default", StringComparison.OrdinalIgnoreCase))
            {
                context.Trace.Writeline(Texts.Hey___you_haven_t_changed_the_the__project_unique_id____wwwroot__or__hostname__in_the___0___configuration_file_, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var dataFolder = PathHelper.Combine(wwwroot, "Data");
            if (!context.FileSystem.DirectoryExists(dataFolder))
            {
                context.Trace.Writeline(Texts.There_Is_No_Data_Directory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
                context.IsAborted = true;
                return;
            }

            var websiteDirectory = PathHelper.Combine(wwwroot, "Website");
            if (!context.FileSystem.DirectoryExists(websiteDirectory))
            {
                context.Trace.Writeline(Texts.There_Is_No_Website_Directory, context.Configuration.Get(Constants.Configuration.ProjectConfigFileName));
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

            PipelineService.Resolve<BeforeBuildPipeline>().Execute(context);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
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
