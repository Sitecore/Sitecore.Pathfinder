// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Pipelines.BeforeBuildPipelines;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class BeforeBuild : BuildTaskBase
    {
        [ImportingConstructor]
        public BeforeBuild([NotNull] IFileSystemService fileSystem, [NotNull] IPipelineService pipelineService, [ImportMany, NotNull, ItemNotNull] IEnumerable<IExtension> extensions) : base("before-build")
        {
            FileSystem = fileSystem;
            PipelineService = pipelineService;
            Extensions = extensions;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IExtension> Extensions { get; }

        [NotNull]
        protected IPipelineService PipelineService { get; }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.Project.ProjectDirectory;
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

            UpdateWebsite(context);

            PipelineService.Resolve<BeforeBuildPipeline>().Execute(context);
        }

        protected virtual bool UpdateExtensions([NotNull] IBuildContext context)
        {
            var updated = false;

            foreach (var extension in Extensions)
            {
                updated |= extension.UpdateWebsiteFiles(context);
            }

            return updated;
        }

        protected virtual void UpdateWebsite([NotNull] IBuildContext context)
        {
            var updated = false;

            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\website");

            var coreServerAssemblyFileName = Path.Combine(context.WebsiteDirectory, "bin\\Sitecore.Pathfinder.Core.dll");
            if (!FileSystem.FileExists(coreServerAssemblyFileName))
            {
                FileSystem.XCopy(sourceDirectory, context.WebsiteDirectory);
                updated = true;
            }
            else
            {
                updated |= UpdateWebsiteFiles(context, sourceDirectory, context.WebsiteDirectory);
            }

            updated |= UpdateWebsiteAssembly(context, "Sitecore.Pathfinder.Core.dll");
            updated |= UpdateWebsiteAssembly(context, "Sitecore.Pathfinder.Roslyn.dll");
            updated |= UpdateWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.dll");
            updated |= UpdateWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.Interfaces.dll");
            updated |= UpdateWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.Json.dll");
            updated |= UpdateWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.Xml.dll");
            updated |= UpdateWebsiteAssembly(context, "ZetaLongPaths.dll");

            updated |= UpdateExtensions(context);

            if (updated)
            {
                context.Trace.WriteLine(Texts.Just_so_you_know__I_have_updated_the__Sitecore_Pathfinder_Server_dll__and__NuGet_Core_dll__assemblies_in_the___bin__directory_in_the_website_and_a_number_of___aspx__files_in_the___sitecore_shell_client_Applications_Pathfinder__directory_to_the_latest_version);
            }
        }

        protected virtual bool UpdateWebsiteAssembly([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            var sourceFileName = Path.Combine(context.ToolsDirectory, fileName);
            var targetFileName = Path.Combine(context.WebsiteDirectory + "\\bin", fileName);

            return FileSystem.CopyIfNewer(sourceFileName, targetFileName);
        }

        protected virtual bool UpdateWebsiteFiles([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string websiteDirectory)
        {
            var updated = false;

            foreach (var sourceFileName in FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories))
            {
                var targetFileName = PathHelper.RemapDirectory(sourceFileName, sourceDirectory, websiteDirectory);
                updated |= FileSystem.CopyIfNewer(sourceFileName, targetFileName);
            }

            return updated;
        }
    }
}
