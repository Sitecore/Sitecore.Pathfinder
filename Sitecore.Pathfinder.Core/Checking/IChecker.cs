namespace Sitecore.Pathfinder.Checking
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IChecker
  {
    void Check([NotNull] ICheckerContext context);
  }
}
