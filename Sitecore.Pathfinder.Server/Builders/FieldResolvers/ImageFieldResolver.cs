namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class ImageFieldResolver : FieldResolverBase
  {
    public override bool CanHandle(IEmitContext context, Field field, Sitecore.Data.Items.Item item)
    {
      var f = item.Fields[field.Name];
      if (f == null)
      {
        return false;
      }

      return string.Compare(f.Type, "image", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override void Handle(IEmitContext context, Field field, Sitecore.Data.Items.Item item)
    {
      if (!field.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      var mediaItem = item.Database.GetItem(field.Value);
      if (mediaItem == null)
      {
        throw new RetryableBuildException(Texts.Text2021, field.TextSpan, field.Value);
      }

      field.Value = $"<image mediapath=\"\" alt=\"Vista15\" width=\"\" height=\"\" hspace=\"\" vspace=\"\" showineditor=\"\" usethumbnail=\"\" src=\"\" mediaid=\"{mediaItem.ID}\" />";
    }
  }
}
