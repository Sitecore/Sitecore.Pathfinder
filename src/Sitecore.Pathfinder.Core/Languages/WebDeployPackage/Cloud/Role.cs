// © 2017 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    using System.Collections.Generic;

    public class Role
    {
        [NotNull]
        public string Name { get; set; }

        [ItemNotNull, NotNull]
        public IEnumerable<string> Membership { get; set; }
    }
}
