namespace Sitecore.Pathfinder.Builders.FieldResolvers
{
  using Sitecore.Data.Templates;
  using Sitecore.Pathfinder.Emitters;
  using Sitecore.Pathfinder.Projects.Items;

  public abstract class FieldResolverBase : IFieldResolver
  {
    public abstract bool CanResolve(IEmitContext context, TemplateField templateField, Field field);

    public abstract string Resolve(IEmitContext context, TemplateField templateField, Field field);
  }
}
