// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Json
{
    public class JsonInnerTextNode : ITextNode
    {
        [NotNull]
        private readonly JToken _token;

        private string _value;

        public JsonInnerTextNode([NotNull] JsonTextNode textNode, [NotNull] JToken token)
        {
            Parent = textNode;
            _token = token; 
        }

        public IEnumerable<ITextNode> Attributes => Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes => Enumerable.Empty<ITextNode>();

        public string Name { get; } = string.Empty;

        public ITextNode Parent { get; }

        public TextPosition Position => Parent?.Position ?? TextPosition.Empty;

        public ISnapshot Snapshot => Parent?.Snapshot ?? Snapshots.Snapshot.Empty;

        public string Value => _value ?? (_value = _token.ToString().Trim());

        public ITextNode GetAttributeTextNode(string attributeName)
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

        public bool SetName(string newName)
        {
            return false;
        }

        public bool SetValue(string value)
        {
            _value = value;

            // todo: make writable
            // _token.Value = value;
            return true;
        }
    }
}
