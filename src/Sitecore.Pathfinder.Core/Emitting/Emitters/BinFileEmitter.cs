// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.BinFiles;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    public class BinFileEmitter : EmitterBase
    {
        public BinFileEmitter() : base(Constants.Emitters.BinFiles)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is BinFile;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var binFile = (BinFile)projectItem;

            var filePath = PathHelper.NormalizeFilePath(binFile.FilePath);
            if (filePath.StartsWith("~\\"))
            {
                filePath = filePath.Mid(2);
            }

            var destinationFileName = PathHelper.Combine(context.Configuration.GetWebsiteDirectory(), filePath);

            if (!CanCopyBinFile(context, binFile, destinationFileName))
            {
                return;
            }

            context.Trace.TraceInformation(Msg.I1011, "Installing bin file", filePath);

            context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName));
            context.FileSystem.Copy(projectItem.Snapshot.SourceFile.AbsoluteFileName, destinationFileName, context.ForceUpdate);
        }

        private bool CanCopyBinFile([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] IProjectItem projectItem, [Diagnostics.NotNull] string destinationFileName)
        {
            if (!context.FileSystem.FileExists(destinationFileName))
            {
                return true;
            }

            if (context.Configuration.GetBool(Constants.Configuration.InstallPackage.CheckBinFileVersion))
            {
                var destinationVersion = GetFileVersion(destinationFileName);
                var sourceVersion = GetFileVersion(projectItem.Snapshot.SourceFile.AbsoluteFileName);

                if (sourceVersion <= destinationVersion)
                {
                    return false;
                }
            }

            if (context.Configuration.GetBool(Constants.Configuration.InstallPackage.CheckBinFileSizeAndTimestamp, true))
            {
                var destinationFileInfo = new FileInfo(destinationFileName);
                var sourceFileInfo = new FileInfo(projectItem.Snapshot.SourceFile.AbsoluteFileName);

                if (sourceFileInfo.Length == destinationFileInfo.Length && sourceFileInfo.LastWriteTimeUtc <= destinationFileInfo.LastWriteTimeUtc)
                {
                    return false;
                }
            }

            return true;
        }

        [Diagnostics.NotNull]
        private Version GetFileVersion([Diagnostics.NotNull] string fileName)
        {
            var info = FileVersionInfo.GetVersionInfo(fileName);
            return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
        }
    }
}
