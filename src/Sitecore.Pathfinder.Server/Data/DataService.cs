// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using Sitecore.Configuration;
using Sitecore.Data;

namespace Sitecore.Pathfinder.Data
{
    [Export(typeof(IDataService))]
    public class DataService : IDataService
    {
        public virtual Database GetDatabase(string databaseName)
        {
            return Factory.GetDatabase(databaseName);
        }
    }
}
