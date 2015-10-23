// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Sitecore.Pathfinder.Snapshots.Directives
{
    [Export(typeof(ISnapshotDirective))]
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
            return snapshotParseContext.InnerTextNodes.TryGetValue(key, out textNodes) ? textNodes : Enumerable.Empty<ITextNode>();
        }
    }
}
