namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Data.Items;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Models.Items;

  public interface IFieldResolver
  {
    bool CanHandle([NotNull] IEmitContext context, [NotNull] FieldModel fieldModel, [NotNull] Item item);

    void Handle([NotNull] IEmitContext context, [NotNull] FieldModel fieldModel, [NotNull] Item item);
  }
}
