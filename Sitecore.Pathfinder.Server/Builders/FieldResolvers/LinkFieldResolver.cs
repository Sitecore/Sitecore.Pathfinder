namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Models.Items;

  [Export(typeof(IFieldResolver))]
  public class LinkFieldResolver : FieldResolverBase
  {
    public override bool CanHandle(IEmitContext context, FieldModel fieldModel, Item item)
    {
      var field = item.Fields[fieldModel.Name];
      if (field == null)
      {
        return false;
      }

      return string.Compare(field.Type, "general link", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(field.Type, "link", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override void Handle(IEmitContext context, FieldModel fieldModel, Item item)
    {
      if (!fieldModel.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      var targetItem = item.Database.GetItem(fieldModel.Value);
      if (targetItem == null)
      {
        throw new RetryableBuildException(Texts.Text2030, fieldModel.SourceFileName, fieldModel.SourceElement, fieldModel.Value);
      }

      fieldModel.Value = $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{targetItem.ID}\" />";
    }
  }
}
