// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Parsing.Builders
{
    public class LanguageVersionBuilder
    {
        [NotNull]
        public string Language { get; set; } = string.Empty;

        [NotNull]
        public ITextNode LanguageTextNode { get; set; } = TextNode.Empty;

        public int Version { get; set; }

        [NotNull]
        public ITextNode VersionTextNode { get; set; } = TextNode.Empty;
    }
}
