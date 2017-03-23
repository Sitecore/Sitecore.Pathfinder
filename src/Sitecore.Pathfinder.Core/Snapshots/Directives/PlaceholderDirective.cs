// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    [Export(typeof(ISnapshotDirective)), Shared]
    public class PlaceholderDirective : SnapshotDirectiveBase
    {
        public override bool CanParse(ITextNode textNode)
        {
            return textNode.Key == "File.Placeholder";
        }

        public override IEnumerable<ITextNode> Parse(SnapshotParseContext snapshotParseContext, ITextNode textNode)
        {
            var key = textNode.GetAttributeValue("Key");
            if (string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(textNode.Value))
            {
                key = textNode.Value;
            }

            if (string.IsNullOrEmpty(key))
            {
                key = string.Empty;
            }

            List<ITextNode> textNodes;
            return snapshotParseContext.PlaceholderTextNodes.TryGetValue(key, out textNodes) ? textNodes : Enumerable.Empty<ITextNode>();
        }
    }
}
