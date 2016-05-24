// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CleanWebsite : BuildTaskBase
    {
        [ImportingConstructor]
        public CleanWebsite([NotNull] IFileSystemService fileSystem, [ImportMany, NotNull, ItemNotNull] IEnumerable<IExtension> extensions) : base("clean-website")
        {
            FileSystem = fileSystem;
            Extensions = extensions;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        [NotNull, ItemNotNull]
        protected IEnumerable<IExtension> Extensions { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.D1022, Texts.Cleaning_website___);

            if (!IsProjectConfigured(context))
            {
                return;
            }

            // todo: remove files from Data Folder as well

            RemoveWebsiteFiles(context);

            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Core.dll");
            RemoveWebsiteAssembly(context, "Sitecore.Pathfinder.Roslyn.dll");
            RemoveWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.dll");
            RemoveWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.Interfaces.dll");
            RemoveWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.Json.dll");
            RemoveWebsiteAssembly(context, "Microsoft.Framework.ConfigurationModel.Xml.dll");

            foreach (var extension in Extensions)
            {
                extension.RemoveWebsiteFiles(context);
            }
        }

        protected virtual void DeleteFile([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            try
            {
                FileSystem.DeleteFile(fileName);
                context.Trace.TraceInformation(Texts.Removed__ + PathHelper.UnmapPath(context.WebsiteDirectory, fileName));
            }
            catch
            {
                context.Trace.TraceInformation(Texts.Failed_to_remove__ + PathHelper.UnmapPath(context.WebsiteDirectory, fileName));
            }
        }

        protected virtual void RemoveWebsiteAssembly([NotNull] IBuildContext context, [NotNull] string assemblyFileName)
        {
            var websiteFileName = Path.Combine(context.WebsiteDirectory, "bin\\" + assemblyFileName);

            if (!FileSystem.FileExists(websiteFileName))
            {
                return;
            }

            DeleteFile(context, websiteFileName);
        }

        protected virtual void RemoveWebsiteFiles([NotNull] IBuildContext context)
        {
            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\website");

            foreach (var fileName in FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories))
            {
                var websiteFileName = PathHelper.RemapDirectory(fileName, sourceDirectory, context.WebsiteDirectory);
                DeleteFile(context, websiteFileName);
            }
        }
    }
}
