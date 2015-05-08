namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  public interface IFieldResolver
  {
    bool CanHandle([NotNull] IEmitContext context, [NotNull] Field field, [NotNull] Sitecore.Data.Items.Item item);

    void Handle([NotNull] IEmitContext context, [NotNull] Field field, [NotNull] Sitecore.Data.Items.Item item);
  }
}
