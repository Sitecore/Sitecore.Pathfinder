// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Extensibility
{
    [InheritedExport(typeof(IExtension))]
    public abstract class ExtensionBase : IExtension
    {
        public abstract void RemoveWebsiteFiles(IBuildContext context);

        public abstract bool UpdateWebsiteFiles(IBuildContext context);

        protected virtual bool CopyProjectFileToWebsiteBinDirectory([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            var projectDirectory = context.Project.ProjectDirectory;
            var websiteDirectory = context.Configuration.GetWebsiteDirectory();

            var sourceFileName = PathHelper.Combine(projectDirectory, fileName);
            var targetFileName = PathHelper.Combine(PathHelper.Combine(websiteDirectory, "bin"), Path.GetFileName(fileName));

            return context.FileSystem.CopyIfNewer(sourceFileName, targetFileName);
        }

        protected virtual bool CopyToolsFileToWebsiteBinDirectory([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            var toolsDirectory = context.ToolsDirectory;
            var websiteDirectory = context.WebsiteDirectory;

            var sourceFileName = PathHelper.Combine(toolsDirectory, fileName);
            var targetFileName = PathHelper.Combine(PathHelper.Combine(websiteDirectory, "bin"), Path.GetFileName(fileName));

            return context.FileSystem.CopyIfNewer(sourceFileName, targetFileName);
        }

        protected virtual void RemoveWebsiteAssembly([NotNull] IBuildContext context, [NotNull] string assemblyFileName)
        {
            var fileName = Path.Combine(context.WebsiteDirectory, "bin\\" + assemblyFileName);

            if (!context.FileSystem.FileExists(fileName))
            {
                return;
            }

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
    }
}
