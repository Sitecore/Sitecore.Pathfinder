// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonInnerTextNode : ITextNode
    {
        [NotNull, ItemNotNull]
        private readonly JToken _token;

        [CanBeNull]
        private string _value;

        public JsonInnerTextNode([NotNull] JsonTextNode textNode, [NotNull, ItemNotNull]  JToken token)
        {
            TextNode = textNode;
            _token = token;
        }

        public IEnumerable<ITextNode> Attributes => Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes => Enumerable.Empty<ITextNode>();

        public string Key { get; } = string.Empty;

        public ISnapshot Snapshot => TextNode.Snapshot;

        public TextSpan TextSpan => TextNode.TextSpan;

        public string Value => _value ?? (_value = _token.ToString().Trim());

        [NotNull]
        protected ITextNode TextNode { get; }

        public ITextNode Inner => null;
    }
}
