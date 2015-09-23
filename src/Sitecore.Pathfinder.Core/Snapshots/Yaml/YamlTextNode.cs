// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Yaml
{
    public class YamlTextNode : TextNode
    {
        public YamlTextNode([NotNull] ISnapshot snapshot, TextSpan span, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent) : base(snapshot, span, name, value, parent)
        {
        }
    }
}
