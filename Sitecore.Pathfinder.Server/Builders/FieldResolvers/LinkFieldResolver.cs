namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Configuration;
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class LinkFieldResolver : FieldResolverBase
  {
    public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
    {
      return string.Compare(templateField.Type, "general link", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(templateField.Type, "link", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
    {
      if (!field.Value.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase))
      {
        return field.Value;
      }

      var database = Factory.GetDatabase(field.Item.DatabaseName);
      var targetItem = database.GetItem(field.Value);
      if (targetItem == null)
      {
        throw new RetryableEmitException(Texts.Item_not_found, field.ValueProperty.TextNode, field.Value);
      }

      return $"<link text=\"\" linktype=\"internal\" url=\"\" anchor=\"\" title=\"\" class=\"\" target=\"\" querystring=\"\" id=\"{targetItem.ID}\" />";
    }
  }
}
