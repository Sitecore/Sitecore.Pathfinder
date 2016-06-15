// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class LanguageVersionContext : ILockable
    {
        public LanguageVersionContext()
        {
            LanguageProperty = new SourceProperty<string>(this, "Language", string.Empty);
            VersionProperty = new SourceProperty<int>(this, "Number", 0);
        }

        Locking ILockable.Locking => Locking.ReadWrite;

        [NotNull]
        public SourceProperty<string> LanguageProperty { get; }

        [NotNull]
        public SourceProperty<int> VersionProperty { get; }
    }
}
