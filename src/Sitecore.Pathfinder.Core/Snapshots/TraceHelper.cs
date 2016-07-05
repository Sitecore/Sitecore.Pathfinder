// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public static class TraceHelper
    {
        [NotNull]
        public static ITextNode GetTextNode([NotNull, ItemCanBeNull]  params IHasSourceTextNodes[] sourceTextNodes)
        {
            foreach (var sourceTextNode in sourceTextNodes)
            {
                if (sourceTextNode == null)
                {
                    continue;
                }

                return sourceTextNode.SourceTextNode;
            }

            return TextNode.Empty;
        }
    }
}
