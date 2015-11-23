// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class DeviceBuilder
    {
        public DeviceBuilder([NotNull] LayoutBuilder layoutBuilder)
        {
            LayoutBuilder = layoutBuilder;
        }

        [NotNull]
        public string DeviceName { get; set; }

        [NotNull]
        public LayoutBuilder LayoutBuilder { get; }

        [NotNull]
        public string LayoutItemPath { get; set; }

        [NotNull]
        [ItemNotNull]
        public IList<RenderingBuilder> Renderings { get; } = new List<RenderingBuilder>();
    }
}
