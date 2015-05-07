namespace Sitecore.Pathfinder.Emitters.ContentFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Diagnostics;
  using Sitecore.IO;
  using Sitecore.Pathfinder.Models;
  using Sitecore.Pathfinder.Models.ContentFiles;

  [Export(typeof(IEmitter))]
  public class ContentFileEmitter : EmitterBase
  {
    public ContentFileEmitter() : base(BinFiles)
    {
    }

    public override bool CanEmit(IEmitContext context, ModelBase model)
    {
      return model is ContentFileModel;
    }

    public override void Emit(IEmitContext context, ModelBase model)
    {
      var contentFileModel = (ContentFileModel)model;
      var destinationFileName = FileUtil.MapPath(contentFileModel.DestinationFileName);

      // todo: backup to uninstall folder
      try
      {
        context.FileSystem.CreateDirectory(Path.GetDirectoryName(destinationFileName));
        context.FileSystem.Copy(contentFileModel.SourceFileName, destinationFileName);
      }
      catch (Exception ex)
      {
        Log.Error($"Failed to copy assembly: {contentFileModel.SourceFileName} -> {destinationFileName}", ex);
      }
    }
  }
}
