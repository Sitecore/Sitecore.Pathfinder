// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Json
{
    public class JsonInnerTextNode : ITextNode, IMutableTextNode
    {
        [NotNull]
        [ItemNotNull]
        private readonly JToken _token;

        [CanBeNull]
        private string _value;

        public JsonInnerTextNode([NotNull] JsonTextNode textNode, [NotNull] [ItemNotNull] JToken token)
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

        ICollection<ITextNode> IMutableTextNode.AttributeCollection { get; } = Constants.EmptyReadOnlyTextNodeCollection;

        ICollection<ITextNode> IMutableTextNode.ChildNodeCollection { get; } = Constants.EmptyReadOnlyTextNodeCollection;

        public ITextNode GetAttribute(string attributeName)
        {
            return null;
        }

        public string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            return string.Empty;
        }

        public ITextNode GetSnapshotLanguageSpecificChildNode(string name)
        {
            return null;
        }

        public bool HasAttribute(string attributeName)
        {
            return false;
        }

        public ITextNode GetInnerTextNode()
        {
            return null;
        }

        bool IMutableTextNode.SetKey(string newKey)
        {
            return false;
        }

        bool IMutableTextNode.SetValue(string newValue)
        {
            _value = newValue;

            // todo: make writable
            // _token.Value = value;
            return true;
        }
    }
}
