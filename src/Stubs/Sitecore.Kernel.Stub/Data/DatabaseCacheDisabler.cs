// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Common;

namespace Sitecore.Data
{
    public class DatabaseCacheDisabler : Switcher<bool, DatabaseCacheDisabler>
    {
        public DatabaseCacheDisabler() : base(true)
        {
        }
    }
}
