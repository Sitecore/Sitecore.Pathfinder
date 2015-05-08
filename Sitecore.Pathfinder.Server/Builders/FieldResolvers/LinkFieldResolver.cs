namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class LinkFieldResolver : FieldResolverBase
  {
    public override bool CanHandle(IEmitContext context, Field field, Sitecore.Data.Items.Item item)
    {
      var f = item.Fields[field.Name];
      if (f == null)
      {
        return false;
      }

      return string.Compare(f.Type, "general link", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(f.Type, "link", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override void Handle(IEmitContext context, Field field, Sitecore.Data.Items.Item item)
    {
      if (!field.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      var targetItem = item.Database.GetItem(field.Value);
      if (targetItem == null)
      {
        throw new RetryableBuildException(Texts.Text2030, field.SourceFile.SourceFileName, field.SourceElement, field.Value);
      }

      field.Value = $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{targetItem.ID}\" />";
    }
  }
}
