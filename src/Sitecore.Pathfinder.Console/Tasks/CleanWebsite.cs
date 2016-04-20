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
        public CleanWebsite([ImportMany, NotNull, ItemNotNull] IEnumerable<IExtension> extensions) : base("clean-website")
        {
            Extensions = extensions;
        }

        [NotNull, ItemNotNull]
        protected IEnumerable<IExtension> Extensions { get; }

        public override void Run(IBuildContext context)
        {
            context.IsAborted = true;
            context.Trace.TraceInformation(Msg.D1022, Texts.Cleaning_website___);

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

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Removes Pathfinder files and assemblies from the website.");
        }

        protected virtual void DeleteFile([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            try
            {
                context.FileSystem.DeleteFile(fileName);
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

            if (!context.FileSystem.FileExists(websiteFileName))
            {
                return;
            }

            DeleteFile(context, websiteFileName);
        }

        protected virtual void RemoveWebsiteFiles([NotNull] IBuildContext context)
        {
            var sourceDirectory = Path.Combine(context.ToolsDirectory, "files\\website");

            foreach (var fileName in context.FileSystem.GetFiles(sourceDirectory, SearchOption.AllDirectories))
            {
                var websiteFileName = PathHelper.RemapDirectory(fileName, sourceDirectory, context.WebsiteDirectory);
                DeleteFile(context, websiteFileName);
            }
        }
    }
}
