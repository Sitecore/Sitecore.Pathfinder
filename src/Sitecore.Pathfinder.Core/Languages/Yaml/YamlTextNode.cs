// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlTextNode : TextNode
    {
        public YamlTextNode([NotNull] ISnapshot snapshot, TextSpan textSpan, [NotNull] string key, [NotNull] string value, [ItemNotNull, NotNull] IEnumerable<ITextNode> attributes, [ItemNotNull, NotNull] IEnumerable<ITextNode> childNodes) : base(snapshot, key, value, textSpan, attributes, childNodes)
        {
        }

        public YamlTextNode([NotNull] ISnapshot snapshot, TextSpan textSpan, [NotNull] string key, [NotNull] string value) : base(snapshot, key, value, textSpan)
        {
        }
    }
}
