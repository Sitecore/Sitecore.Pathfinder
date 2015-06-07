namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  public interface IFieldResolver
  {
    bool CanResolve([NotNull] IEmitContext context, [NotNull] TemplateField templateField, [NotNull] Field field);

    [NotNull]
    string Resolve([NotNull] IEmitContext context, [NotNull] TemplateField templateField, [NotNull] Field field);
  }
}
