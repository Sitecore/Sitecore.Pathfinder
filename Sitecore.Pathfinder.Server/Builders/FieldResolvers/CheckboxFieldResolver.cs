namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  [Export(typeof(IFieldResolver))]
  public class CheckboxFieldResolver : FieldResolverBase
  {
    public override bool CanResolve(IEmitContext context, TemplateField templateField, Field field)
    {
      return string.Compare(templateField.Type, "checkbox", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override string Resolve(IEmitContext context, TemplateField templateField, Field field)
    {
      if (string.Compare(field.Value.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)
      {
        return "1";
      }

      if (string.Compare(field.Value.Value, "1", StringComparison.OrdinalIgnoreCase) == 0)
      {
        return "1";
      }

      return string.Empty;
    }
  }
}
