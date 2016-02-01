// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Jobs;
using Sitecore.SecurityModel;

namespace Sitecore
{
    public static class Context
    {
        [NotNull]
        public static Item Item { get; set; }

        [CanBeNull]
        public static User User{ get; private set; }

        [CanBeNull]
        public static Job Job { get; private set; }
    }
}
