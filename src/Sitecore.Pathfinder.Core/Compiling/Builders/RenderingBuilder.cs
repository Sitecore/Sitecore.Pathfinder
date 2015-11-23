// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Compiling.Builders
{
    public class RenderingBuilder
    {
        public RenderingBuilder([NotNull] DeviceBuilder deviceBuilder)
        {
            DeviceBuilder = deviceBuilder;
        }

        [NotNull]
        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();

        public bool Cacheable { get; set; }

        [NotNull]
        public string DataSource { get; set; } = string.Empty;

        [NotNull]
        public DeviceBuilder DeviceBuilder { get; }

        [NotNull]
        public string Id { get; set; } = string.Empty;

        [NotNull]
        public string Name { get; set; } = string.Empty;

        [CanBeNull]
        public RenderingBuilder ParentRendering { get; set; }

        [NotNull]
        public string Placeholder { get; set; } = string.Empty;

        [NotNull]
        [ItemNotNull]
        public List<string> Placeholders { get; } = new List<string>();

        public bool UnsafeName { get; set; }

        public bool VaryByData { get; set; }

        public bool VaryByDevice { get; set; }

        public bool VaryByLogin { get; set; }

        public bool VaryByParameters { get; set; }

        public bool VaryByQueryString { get; set; }

        public bool VaryByUser { get; set; }
    }
}
