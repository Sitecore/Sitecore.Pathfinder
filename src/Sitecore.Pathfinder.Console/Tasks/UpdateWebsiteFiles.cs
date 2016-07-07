// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class UpdateWebsiteFiles : BuildTaskBase, IPreRunTask, ISetupTask
    {
        [ImportingConstructor]
        public UpdateWebsiteFiles([NotNull] IFileSystemService fileSystem, [ImportMany, NotNull, ItemNotNull] IEnumerable<IExtension> extensions) : base("update-website-files")
        {
            FileSystem = fileSystem;
            Extensions = extensions;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<IExtension> Extensions { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public override void Run(IBuildContext context)
        {
            var projectDirectory = context.ProjectDirectory;
            if (string.IsNullOrEmpty(projectDirectory) || !FileSystem.DirectoryExists(projectDirectory))
            {
                return;
            }

            var websiteDirectory = context.Configuration.GetWebsiteDirectory();
            if (string.IsNullOrEmpty(websiteDirectory))
            {
                return;
            }

            websiteDirectory = PathHelper.Combine(projectDirectory, websiteDirectory);
            if (!FileSystem.DirectoryExists(websiteDirectory))
            {
                return;
            }

            UpdateWebsite(context);
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

        protected virtual bool UpdateFiles([NotNull] IBuildContext context, [NotNull] string sourceDirectory, [NotNull] string websiteDirectory)
        {
            var updated = false;

            foreach (var sourceFileName in FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories))
            {
                var targetFileName = PathHelper.RemapDirectory(sourceFileName, sourceDirectory, websiteDirectory);
                updated |= FileSystem.CopyIfNewer(sourceFileName, targetFileName);
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
                updated |= UpdateFiles(context, sourceDirectory, context.WebsiteDirectory);
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
    }
}
