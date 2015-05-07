namespace Sitecore.Pathfinder.Data
{
  using Sitecore.Data;
  using Sitecore.Pathfinder.Diagnostics;

  public interface IDataService
  {
    [CanBeNull]
    Database GetDatabase([NotNull] string databaseName);
  }
}
