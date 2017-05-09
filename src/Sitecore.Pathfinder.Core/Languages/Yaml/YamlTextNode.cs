// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlTextNode : TextNode, IMutableTextNode
    {
        public YamlTextNode([NotNull] ISnapshot snapshot, TextSpan textSpan, [NotNull] string key, [NotNull] string value) : base(snapshot, key, value, textSpan)
        {
        }

        ICollection<ITextNode> IMutableTextNode.AttributeCollection => (IList<ITextNode>)Attributes;

        ICollection<ITextNode> IMutableTextNode.ChildNodeCollection => (IList<ITextNode>)ChildNodes;

        bool IMutableTextNode.SetKey(string newKey)
        {
            return false;
        }

        bool IMutableTextNode.SetValue(string newValue)
        {
            return false;
        }
    }
}
