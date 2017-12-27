// © 2015-2017 by Jakob Christensen. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.Items
{
    public class VersionedItem : Item
    {
        public VersionedItem([NotNull] IDatabase database, Guid guid, [NotNull] string itemName, [NotNull] string itemIdOrPath, [NotNull] string templateIdOrPath, [NotNull] Language language, [NotNull] Version version) : base(database, guid, itemName, itemIdOrPath, templateIdOrPath)
        {
            Language = language;
            Version = version;
        }

        [NotNull]
        public Language Language { get; }

        // todo: this is not right
        public override Locking Locking => Locking.ReadWrite;

        [NotNull]
        public Version Version { get; }
    }
}
