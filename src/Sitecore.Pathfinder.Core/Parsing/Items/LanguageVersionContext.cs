// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Parsing.Items
{
    public class LanguageVersionContext : ILockable
    {
        public LanguageVersionContext()
        {
            LanguageProperty = new SourceProperty<Language>(this, "Language", Language.Undefined);
            VersionProperty = new SourceProperty<Version>(this, "Number", Version.Undefined);
        }

        [NotNull]
        public SourceProperty<Language> LanguageProperty { get; }

        [NotNull]
        public SourceProperty<Version> VersionProperty { get; }

        Locking ILockable.Locking => Locking.ReadWrite;
    }
}
