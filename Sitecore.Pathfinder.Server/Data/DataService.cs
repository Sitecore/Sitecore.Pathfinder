namespace Sitecore.Pathfinder.Data
{
  using System.ComponentModel.Composition;
  using Sitecore.Configuration;
  using Sitecore.Data;

  [Export(typeof(IDataService))]
  public class DataService : IDataService
  {
    public virtual Database GetDatabase(string databaseName)
    {
      return Factory.GetDatabase(databaseName);
    }
  }
}
