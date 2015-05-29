namespace Sitecore.Pathfinder.Data
{
  using System.ComponentModel.Composition;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;

  [Export(typeof(IDataService))]
  public class DataService : IDataService
  {
    public virtual Database GetDatabase(string databaseName)
    {
      return Factory.GetDatabase(databaseName);
    }
  }
}
