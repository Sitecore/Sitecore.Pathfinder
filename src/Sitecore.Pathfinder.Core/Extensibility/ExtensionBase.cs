// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using Sitecore.Pathfinder.Building;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Extensibility
{
    public abstract class ExtensionBase : IExtension
    {
        public abstract void UpdateWebsiteFiles(IBuildContext context);

        protected virtual void CopyToWebsiteBinDirectory([NotNull] IBuildContext context, [NotNull] string fileName)
        {
            var projectDirectory = context.ProjectDirectory;
            var websiteDirectory = context.Configuration.GetString(Constants.Configuration.WebsiteDirectory);

            var sourceFileName = PathHelper.Combine(projectDirectory, fileName);
            var targetFileName = PathHelper.Combine(PathHelper.Combine(websiteDirectory, "bin"), Path.GetFileName(fileName));

            if (!File.Exists(targetFileName))
            {
                context.FileSystem.Copy(sourceFileName, targetFileName);
                return;
            }

            // check version and size before copying
            var sourceVersion = new Version(FileVersionInfo.GetVersionInfo(sourceFileName).FileVersion);
            var targetVersion = new Version(FileVersionInfo.GetVersionInfo(targetFileName).FileVersion);
            if (targetVersion < sourceVersion)
            {
                context.FileSystem.Copy(sourceFileName, targetFileName);
                return;
            }

            var sourceFileInfo = new FileInfo(sourceFileName);
            var targetFileInfo = new FileInfo(targetFileName);
            if (sourceFileInfo.Length != targetFileInfo.Length)
            {
                context.FileSystem.Copy(sourceFileName, targetFileName);
            }
        }
    }
}
