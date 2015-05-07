namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Models.Items;

  [Export(typeof(IFieldResolver))]
  public class CheckboxFieldResolver : FieldResolverBase
  {
    public override bool CanHandle(IEmitContext context, FieldModel fieldModel, Item item)
    {
      var field = item.Fields[fieldModel.Name];
      if (field == null)
      {
        return false;
      }
      
      return string.Compare(field.Type, "checkbox", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override void Handle(IEmitContext context, FieldModel fieldModel, Item item)
    {
      if (string.Compare(fieldModel.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)
      {
        fieldModel.Value = "1";
        return;
      }

      if (string.Compare(fieldModel.Value, "false", StringComparison.OrdinalIgnoreCase) == 0)
      {
        fieldModel.Value = string.Empty;
      }
    }
  }
}
