using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Extensions
{
    public static class TextNodeExtensions
    {
        [CanBeNull]
        public static ITextNode GetAttribute([NotNull] this ITextNode textNode, [NotNull] string attributeName) => textNode.Attributes.FirstOrDefault(a => a.Key == attributeName);

        public static bool GetAttributeBool([NotNull] this ITextNode textNode, [NotNull] string attributeName, bool defaultValue = false)
        {
            if (!textNode.HasAttribute(attributeName))
            {
                return defaultValue;
            }

            var value = textNode.GetAttributeValue(attributeName);
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }


        [NotNull]
        public static string GetAttributeValue([NotNull] this ITextNode textNode, [NotNull] string attributeName, [NotNull] string defaultValue = "")
        {
            var attribute = textNode.GetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }

            var value = attribute.Value;
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
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

        public static bool HasAttribute([NotNull] this ITextNode textNode, [NotNull] string attributeName) => textNode.GetAttribute(attributeName) != null;
    }
}
