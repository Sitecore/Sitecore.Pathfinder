// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data;

namespace Sitecore.Pathfinder.Data
{
    public interface IDataService
    {
        [Diagnostics.CanBeNull]        
        Database GetDatabase([Diagnostics.NotNull] string databaseName);
    }
}
