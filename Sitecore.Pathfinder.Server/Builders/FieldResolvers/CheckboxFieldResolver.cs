namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class CheckboxFieldResolver : FieldResolverBase
  {
    public override bool CanHandle(IEmitContext context, Field field, Sitecore.Data.Items.Item item)
    {
      var f = item.Fields[field.Name];
      if (f == null)
      {
        return false;
      }

      return string.Compare(f.Type, "checkbox", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override void Handle(IEmitContext context, Field field, Sitecore.Data.Items.Item item)
    {
      if (string.Compare(field.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)
      {
        field.Value = "1";
        return;
      }

      if (string.Compare(field.Value, "false", StringComparison.OrdinalIgnoreCase) == 0)
      {
        field.Value = string.Empty;
      }
    }
  }
}
