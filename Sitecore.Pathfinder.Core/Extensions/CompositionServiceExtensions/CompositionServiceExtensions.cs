namespace Sitecore.Pathfinder.Extensions.CompositionServiceExtensions
{
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;
  using Sitecore.Pathfinder.Diagnostics;

  public static class CompositionServiceExtensions
  {
    public static T Resolve<T>([NotNull] this ICompositionService compositionService)
    {
      var compositionContainer = (CompositionContainer)compositionService;

      return compositionContainer.GetExportedValue<T>();
    }
  }
}
