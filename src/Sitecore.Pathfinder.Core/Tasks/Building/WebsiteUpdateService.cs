// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Tasks.Building
{
    [Export(typeof(IWebsiteUpdateService))]
    public class WebsiteUpdateService : IWebsiteUpdateService
    {
        private bool _isUpdated;

        [ImportingConstructor]
        public WebsiteUpdateService([NotNull] IConfiguration configuration, [NotNull] ICompositionService compositionService, [NotNull] ITraceService trace,  [NotNull] IFileSystemService fileSystem, [ImportMany, NotNull, ItemNotNull] IEnumerable<IExtension> extensions)
        {
            Configuration = configuration;
            CompositionService = compositionService;
            Trace = trace;
            FileSystem = fileSystem;
            Extensions = extensions;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected ICompositionService CompositionService { get; }

        [NotNull]
        public ITraceService Trace { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IExtension> Extensions { get; }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public void UpdateWebsiteFiles()
        {
            if (_isUpdated)
            {
                return;
            }

            var projectDirectory = Configuration.GetProjectDirectory();
            if (string.IsNullOrEmpty(projectDirectory) || !FileSystem.DirectoryExists(projectDirectory))
            {
                return;
            }

            var websiteDirectory = Configuration.GetWebsiteDirectory();
            if (string.IsNullOrEmpty(websiteDirectory))
            {
                return;
            }

            websiteDirectory = PathHelper.Combine(projectDirectory, websiteDirectory);
            if (!FileSystem.DirectoryExists(websiteDirectory))
            {
                return;
            }

            UpdateWebsite();

            _isUpdated = true;
        }

        protected virtual bool UpdateExtensions()
        {
            var context = CompositionService.Resolve<IExtensionContext>();

            var updated = false;
            foreach (var extension in Extensions)
            {
                updated |= extension.UpdateWebsiteFiles(context);
            }

            return updated;
        }

        protected virtual bool UpdateFiles([NotNull] string sourceDirectory, [NotNull] string websiteDirectory)
        {
            var updated = false;

            foreach (var sourceFileName in FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories))
            {
                var targetFileName = PathHelper.RemapDirectory(sourceFileName, sourceDirectory, websiteDirectory);
                updated |= FileSystem.CopyIfNewer(sourceFileName, targetFileName);
            }

            return updated;
        }

        protected virtual void UpdateWebsite()
        {
            var updated = false;

            var sourceDirectory = Path.Combine(Configuration.GetToolsDirectory(), "files\\website");

            var coreServerAssemblyFileName = Path.Combine(Configuration.GetWebsiteDirectory(), "bin\\Sitecore.Pathfinder.Core.dll");
            if (!FileSystem.FileExists(coreServerAssemblyFileName))
            {
                FileSystem.XCopy(sourceDirectory, Configuration.GetWebsiteDirectory());
                updated = true;
            }
            else
            {
                updated |= UpdateFiles(sourceDirectory, Configuration.GetWebsiteDirectory());
            }

            updated |= UpdateWebsiteAssembly("Sitecore.Pathfinder.Core.dll");
            updated |= UpdateWebsiteAssembly("Sitecore.Pathfinder.Roslyn.dll");
            updated |= UpdateWebsiteAssembly("ZetaLongPaths.dll");

            updated |= UpdateWebsiteAssembly("files\\extensions\\Sitecore.Pathfinder.Core.Extensions.dll", "Sitecore.Pathfinder.Core.Extensions.dll");

            updated |= UpdateExtensions();

            if (updated)
            {
                Trace.WriteLine(Texts.Just_so_you_know__I_have_updated_the__Sitecore_Pathfinder_Server_dll__and__NuGet_Core_dll__assemblies_in_the___bin__directory_in_the_website_and_a_number_of___aspx__files_in_the___sitecore_shell_client_Applications_Pathfinder__directory_to_the_latest_version);
            }
        }

        protected virtual bool UpdateWebsiteAssembly([NotNull] string fileName, [NotNull] string newFileName = "")
        {
            if (string.IsNullOrEmpty(newFileName))
            {
                newFileName = fileName;
            }

            var sourceFileName = Path.Combine(Configuration.GetToolsDirectory(), fileName);
            var targetFileName = Path.Combine(Configuration.GetWebsiteDirectory() + "\\bin", newFileName);

            return FileSystem.CopyIfNewer(sourceFileName, targetFileName);
        }
    }
}
