// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Extensibility
{
    [InheritedExport(typeof(IExtension))]
    public abstract class ExtensionBase : IExtension
    {
        public virtual void RemoveWebsiteFiles(IExtensionContext context)
        {
        }

        public virtual void Start()
        {
        }

        public virtual bool UpdateWebsiteFiles(IExtensionContext context)
        {
            return false;
        }

        protected virtual bool CopyProjectFileToWebsiteBinDirectory([NotNull] IExtensionContext context, [NotNull] string fileName)
        {
            var projectDirectory = context.Configuration.GetProjectDirectory();
            var websiteDirectory = context.Configuration.GetWebsiteDirectory();

            var sourceFileName = PathHelper.Combine(projectDirectory, fileName);
            var targetFileName = PathHelper.Combine(PathHelper.Combine(websiteDirectory, "bin"), Path.GetFileName(fileName));

            return context.FileSystem.CopyIfNewer(sourceFileName, targetFileName);
        }

        protected virtual bool CopyToolsFileToWebsiteBinDirectory([NotNull] IExtensionContext context, [NotNull] string fileName)
        {
            var toolsDirectory = context.Configuration.GetToolsDirectory();
            var websiteDirectory = context.Configuration.GetWebsiteDirectory();

            var sourceFileName = PathHelper.Combine(toolsDirectory, fileName);
            var targetFileName = PathHelper.Combine(PathHelper.Combine(websiteDirectory, "bin"), Path.GetFileName(fileName));

            return context.FileSystem.CopyIfNewer(sourceFileName, targetFileName);
        }

        protected virtual void RemoveWebsiteAssembly([NotNull] IExtensionContext context, [NotNull] string assemblyFileName)
        {
            var fileName = Path.Combine(context.Configuration.GetWebsiteDirectory(), "bin\\" + assemblyFileName);

            if (!context.FileSystem.FileExists(fileName))
            {
                return;
            }

            try
            {
                context.FileSystem.DeleteFile(fileName);
                context.Trace.TraceInformation(Texts.Removed__ + PathHelper.UnmapPath(context.Configuration.GetWebsiteDirectory(), fileName));
            }
            catch
            {
                context.Trace.TraceInformation(Texts.Failed_to_remove__ + PathHelper.UnmapPath(context.Configuration.GetWebsiteDirectory(), fileName));
            }
        }
    }
}
