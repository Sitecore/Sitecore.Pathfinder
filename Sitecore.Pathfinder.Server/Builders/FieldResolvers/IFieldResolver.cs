namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  public interface IFieldResolver
  {
    bool CanResolve([NotNull] IEmitContext context, [NotNull] Field field, [NotNull] Sitecore.Data.Items.Item item);

    void Resolve([NotNull] IEmitContext context, [NotNull] Field field, [NotNull] Sitecore.Data.Items.Item item);
  }
}
