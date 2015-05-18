namespace Sitecore.Pathfinder.Checking
{
  public abstract class CheckerBase : IChecker
  {
    public abstract void Check(ICheckerContext context);
  }
}
