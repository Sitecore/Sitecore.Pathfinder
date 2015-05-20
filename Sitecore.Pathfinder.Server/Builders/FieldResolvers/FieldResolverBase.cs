namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  public abstract class FieldResolverBase : IFieldResolver
  {
    public abstract bool CanResolve(IEmitContext context, Field field, Sitecore.Data.Items.Item item);

    public abstract void Resolve(IEmitContext context, Field field, Sitecore.Data.Items.Item item);
  }
}
