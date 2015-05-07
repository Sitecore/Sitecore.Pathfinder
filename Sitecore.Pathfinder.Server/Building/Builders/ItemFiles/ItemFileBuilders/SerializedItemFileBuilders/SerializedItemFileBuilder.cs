namespace Sitecore.Pathfinder.Building.Builders.ItemFiles.ItemFileBuilders.SerializedItemFileBuilders
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Data.Items;
  using Sitecore.Data.Serialization;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(IItemFileBuilder))]
  public class SerializedItemFileBuilder : ItemFileBuilderBase
  {
    private const string FileExtension = ".item";

    private static readonly LoadOptions LoadOptions = new LoadOptions
    {
      ForceUpdate = true
    };

    public SerializedItemFileBuilder() : base(Items)
    {
    }

    public override void Build(IItemFileBuildContext context)
    {
      try
      {
        var item = this.DoLoadItem(context.FileName, LoadOptions);
        if (item == null)
        {
          throw new RetryableBuildException(Texts.Text2022, context.FileName);
        }
      }
      catch (Exception ex)
      {
        throw new RetryableBuildException(Texts.Text2022, context.FileName, 0, 0, ex.Message);
      }
    }

    public override bool CanBuild(IItemFileBuildContext context)
    {
      return context.FileName.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase);
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
