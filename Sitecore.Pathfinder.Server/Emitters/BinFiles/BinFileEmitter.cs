namespace Sitecore.Pathfinder.Emitters.BinFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.BinFiles;

  [Export(typeof(IEmitter))]
  public class BinFileEmitter : EmitterBase
  {
    public BinFileEmitter() : base(BinFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectElementBase model)
    {
      return model is BinFile;
    }

    public override void Emit(IEmitContext context, ProjectElementBase model)
    {
      var binFileModel = (BinFile)model;
      var destinationFileName = FileUtil.MapPath(binFileModel.DestinationFileName);

      // todo: check for assembly version
      // todo: backup to uninstall folder
      try
      {
        context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName));
        context.FileSystem.Copy(binFileModel.SourceFileName, destinationFileName);
      }
      catch (Exception ex)
      {
        Log.Error($"Failed to copy assembly: {binFileModel.SourceFileName} -> {destinationFileName}", ex);
      }
    }
  }
}
