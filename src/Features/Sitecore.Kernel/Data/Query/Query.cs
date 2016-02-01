// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;

namespace Sitecore.Data.Query
{
    public class Query
    {
        public int Max { get; set; }

        [CanBeNull]
        public object Execute([NotNull] Item getRootItem)
        {
            return null;
        }
    }
}