namespace Sitecore.Pathfinder.Server.Tests.Data
{
  using Sitecore.Data;
  using Sitecore.Pathfinder.Data;

  public class TestDataService : DataService
  {
    public override Database GetDatabase(string databaseName)
    {
      return null;
    }
  }
}
