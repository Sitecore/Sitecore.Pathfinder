// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class LayoutBuilder
    {
        [NotNull, ItemNotNull]
        public ICollection<DeviceBuilder> Devices { get; } = new List<DeviceBuilder>();
    }
}
