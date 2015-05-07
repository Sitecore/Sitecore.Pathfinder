namespace Sitecore.Pathfinder.Emitters.SerializationFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Data.Items;
  using Sitecore.Data.Serialization;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.SerializationFiles;

  [Export(typeof(IEmitter))]
  public class SerializationFileEmitter : EmitterBase
  {
    private static readonly LoadOptions LoadOptions = new LoadOptions
    {
      ForceUpdate = true
    };

    public SerializationFileEmitter() : base(Items)
    {
    }

    public override bool CanEmit(IEmitContext context, ProjectElementBase model)
    {
      return model is SerializationFile;
    }

    public override void Emit(IEmitContext context, ProjectElementBase model)
    {
      var itemModel = (SerializationFile)model;

      try
      {
        var item = this.DoLoadItem(itemModel.SerializationFile, LoadOptions);
        if (item == null)
        {
          throw new RetryableBuildException(Texts.Text2022, itemModel.SourceFileName);
        }
      }
      catch (Exception ex)
      {
        throw new RetryableBuildException(Texts.Text2022, itemModel.SourceFileName, 0, 0, ex.Message);
      }
    }

    [CanBeNull]
    protected virtual Item DoLoadItem([NotNull] string fileName, [NotNull] LoadOptions options)
    {
      using (var reader = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
      {
        var disabledLocally = ItemHandler.DisabledLocally;
        try
        {
          ItemHandler.DisabledLocally = true;
          return ItemSynchronization.ReadItem(reader, options);
        }
        finally
        {
          ItemHandler.DisabledLocally = disabledLocally;
        }
      }
    }
  }
}
