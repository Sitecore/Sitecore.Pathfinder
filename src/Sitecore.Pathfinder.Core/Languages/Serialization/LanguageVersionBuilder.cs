// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    public class LanguageVersionBuilder
    {
        [NotNull]
        public Language Language { get; set; } = Language.Undefined;

        [NotNull]
        public ITextNode LanguageTextNode { get; set; } = TextNode.Empty;

        [NotNull]
        public Version Version { get; set; } = Version.Undefined;

        [NotNull]
        public ITextNode VersionTextNode { get; set; } = TextNode.Empty;
    }
}
