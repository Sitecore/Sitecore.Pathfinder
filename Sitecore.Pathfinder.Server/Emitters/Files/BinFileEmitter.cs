namespace Sitecore.Pathfinder.Emitters.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;

  [Export(typeof(IEmitter))]
  public class BinFileEmitter : EmitterBase
  {
    public BinFileEmitter() : base(BinFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectItem projectItem)
    {
      return projectItem is BinFile;
    }

    public override void Emit(IEmitContext context, ProjectItem projectItem)
    {
      var binFile = (BinFile)projectItem;

      var destinationFileName = "/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(context.Project.ProjectDirectory, binFile.TextSpan.SourceFileName));
      destinationFileName = FileUtil.MapPath(destinationFileName);

      // todo: check for assembly version
      // todo: backup to uninstall folder
      try
      {
        context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName) ?? string.Empty);
        context.FileSystem.Copy(binFile.TextSpan.SourceFileName, destinationFileName);
      }
      catch (Exception ex)
      {
        Log.Error($"Failed to copy assembly: {binFile.TextSpan.SourceFileName} -> {destinationFileName}", ex);
      }
    }
  }
}
