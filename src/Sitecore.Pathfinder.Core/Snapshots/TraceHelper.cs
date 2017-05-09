// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public static class TraceHelper
    {
        [NotNull]
        public static ITextNode GetTextNode([NotNull, ItemCanBeNull] params IHasSourceTextNodes[] sourceTextNodes)
        {
            foreach (var sourceTextNode in sourceTextNodes)
            {
                if (sourceTextNode == null)
                {
                    continue;
                }

                if (sourceTextNode.SourceTextNode != TextNode.Empty)
                {
                    return sourceTextNode.SourceTextNode;
                }
            }

            return TextNode.Empty;
        }
    }
}
