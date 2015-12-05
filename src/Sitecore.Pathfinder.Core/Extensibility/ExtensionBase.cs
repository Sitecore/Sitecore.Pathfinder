// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Extensibility
{
    public abstract class ExtensionBase : IExtension
    {
        public abstract bool UpdateWebsiteFiles(IBuildContext context);

        protected virtual bool CopyProjectFileToWebsiteBinDirectory([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            var projectDirectory = context.ProjectDirectory;
            var websiteDirectory = context.WebsiteDirectory;

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
    }
}
