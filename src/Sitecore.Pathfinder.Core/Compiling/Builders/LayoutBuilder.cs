// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class LayoutBuilder
    {
        [NotNull]
        [ItemNotNull]
        public IList<DeviceBuilder> Devices { get; } = new List<DeviceBuilder>();
    }
}
