namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class ImageFieldResolver : FieldResolverBase
  {
    public override bool CanHandle(IEmitContext context, FieldModel fieldModel, Item item)
    {
      var field = item.Fields[fieldModel.Name];
      if (field == null)
      {
        return false;
      }

      return string.Compare(field.Type, "image", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override void Handle(IEmitContext context, FieldModel fieldModel, Item item)
    {
      if (!fieldModel.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      var mediaItem = item.Database.GetItem(fieldModel.Value);
      if (mediaItem == null)
      {
        throw new RetryableBuildException(Texts.Text2021, fieldModel.SourceFileName, fieldModel.SourceElement, fieldModel.Value);
      }

      fieldModel.Value = $"<image mediapath=\"\" alt=\"Vista15\" width=\"\" height=\"\" hspace=\"\" vspace=\"\" showineditor=\"\" usethumbnail=\"\" src=\"\" mediaid=\"{mediaItem.ID}\" />";
    }
  }
}
