// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Extensions
{
    public static class TextNodeExtensions
    {
        [ItemNotNull, NotNull]
        public static IEnumerable<ITextNode> GetTextNodes([NotNull] this IHasSourceTextNodes hasSourceTextNodes)
        {
            if (hasSourceTextNodes.SourceTextNode != TextNode.Empty)
            {
                yield return hasSourceTextNodes.SourceTextNode;
            }

            foreach (var textNode in hasSourceTextNodes.AdditionalSourceTextNodes)
            {
                yield return textNode;
            }
        }
    }
}
