// © 2015-2017 by Jakob Christensen. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Unicorn
{
    [Export]
    public class UnicornTextSnapshot : YamlTextSnapshot
    {
        [FactoryConstructor, ImportingConstructor]
        public UnicornTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }
    }
}
