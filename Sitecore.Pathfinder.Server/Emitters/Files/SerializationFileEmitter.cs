namespace Sitecore.Pathfinder.Emitters.Files
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Data.Items;
  using Sitecore.Data.Serialization;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.Projects.Files;

  [Export(typeof(IEmitter))]
  public class SerializationFileEmitter : EmitterBase
  {
    private static readonly LoadOptions LoadOptions = new LoadOptions
    {
      ForceUpdate = true
    };

    public SerializationFileEmitter() : base(Constants.Emitters.Items)
    {
    }

    public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
    {
      return projectItem is SerializationFile;
    }

    public override void Emit(IEmitContext context, IProjectItem projectItem)
    {
      var itemModel = (SerializationFile)projectItem;

      try
      {
        var item = this.DoLoadItem(itemModel.DocumentSnapshot.SourceFile.FileName, LoadOptions);
        if (item == null)
        {
          throw new RetryableEmitException("Failed to upload media", itemModel.DocumentSnapshot, "Item not created");
        }
      }
      catch (Exception ex)
      {
        throw new RetryableEmitException("Failed to upload media", itemModel.DocumentSnapshot, ex.Message);
      }
    }

    [CanBeNull]
    protected virtual Item DoLoadItem([NotNull] string fileName, [NotNull] LoadOptions options)
    {
      using (var reader = new StreamReader(System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
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
