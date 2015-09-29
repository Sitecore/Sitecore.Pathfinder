// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonInnerTextNode : ITextNode
    {
        [NotNull]
        [ItemNotNull]
        private readonly JToken _token;

        [CanBeNull]
        private string _value;

        public JsonInnerTextNode([NotNull] JsonTextNode textNode, [NotNull][ItemNotNull] JToken token)
        {
            ParentNode = textNode;
            _token = token;
        }

        public IEnumerable<ITextNode> Attributes => Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes => Enumerable.Empty<ITextNode>();

        public string Key { get; } = string.Empty;

        public ITextNode ParentNode { get; }

        public TextSpan TextSpan => ParentNode?.TextSpan ?? TextSpan.Empty;

        public ISnapshot Snapshot => ParentNode?.Snapshot ?? Snapshots.Snapshot.Empty;

        public string Value => _value ?? (_value = _token.ToString().Trim());

        public ITextNode GetAttribute(string attributeName)
        {
            return null;
        }

        public string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            return string.Empty;
        }

        public ITextNode GetInnerTextNode()
        {
            return null;
        }

        public bool SetKey(string newKey)
        {
            return false;
        }

        public bool SetValue(string newValue)
        {
            _value = newValue;

            // todo: make writable
            // _token.Value = value;
            return true;
        }
    }
}
