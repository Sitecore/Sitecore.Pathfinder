// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Extensions
{
    public static class TextNodeExtensions
    {
        public static bool GetAttributeBool([NotNull] this ITextNode textNode, [NotNull] string attributeName, bool defaultValue = false)
        {
            if (!textNode.HasAttribute(attributeName))
            {
                return defaultValue;
            }

            var value = textNode.GetAttributeValue(attributeName);
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

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
