namespace Sitecore.Pathfinder.Emitters.BinFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Models;
  using Sitecore.Pathfinder.Models.BinFiles;

  [Export(typeof(IEmitter))]
  public class BinFileEmitter : EmitterBase
  {
    public BinFileEmitter() : base(BinFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, ModelBase model)
    {
      return model is BinFileModel;
    }

    public override void Emit(IEmitContext context, ModelBase model)
    {
      var binFileModel = (BinFileModel)model;
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
