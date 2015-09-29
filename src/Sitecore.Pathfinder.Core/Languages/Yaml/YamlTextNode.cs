// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlTextNode : TextNode
    {
        public YamlTextNode([NotNull] ISnapshot snapshot, TextSpan textSpan, [NotNull] string key, [NotNull] string value, [CanBeNull] ITextNode parentNode) : base(snapshot, key, value, textSpan, parentNode)
        {
        }
    }
}
