namespace Sitecore.Pathfinder.Emitters.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.Diagnostics;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;

  [Export(typeof(IEmitter))]
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
      var destinationFileName = FileUtil.MapPath(binFile.FilePath);

      if (!this.CanCopyBinFile(context, binFile, destinationFileName))
      {
        return;
      }

      if (context.FileSystem.FileExists(destinationFileName))
      {
        context.RegisterUpdatedFile(binFile, destinationFileName);
      }
      else
      {
        context.RegisterAddedFile(binFile, destinationFileName);
      }

      context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
      context.FileSystem.Copy(projectItem.Snapshot.SourceFile.FileName, destinationFileName);
    }

    private bool CanCopyBinFile([NotNull] IEmitContext context, [NotNull] IProjectItem projectItem, [NotNull] string destinationFileName)
    {
      if (!context.FileSystem.FileExists(destinationFileName))
      {
        return true;
      }

      if (!context.Configuration.GetBool(Constants.Configuration.CheckBinFileVersion))
      {
        return true;
      }

      var destinationVersion = this.GetFileVersion(destinationFileName);
      var sourceVersion = this.GetFileVersion(projectItem.Snapshot.SourceFile.FileName);

      return sourceVersion > destinationVersion;
    }

    [NotNull]
    private Version GetFileVersion([NotNull] string fileName)
    {
      var info = FileVersionInfo.GetVersionInfo(fileName);
      return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
    }
  }
}
