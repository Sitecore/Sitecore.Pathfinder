namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Models.Items;

  public abstract class FieldResolverBase : IFieldResolver
  {
    public abstract bool CanHandle(IEmitContext context, FieldModel fieldModel, Item item);

    public abstract void Handle(IEmitContext context, FieldModel fieldModel, Item item);
  }
}
