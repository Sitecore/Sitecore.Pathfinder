// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Xml
{
    public class XmlInnerTextNode : ITextNode
    {
        [NotNull]
        private static readonly Regex RemoveNamespaces = new Regex("\\sxmlns[^\"]+\"[^\"]+\"", RegexOptions.Compiled);

        [NotNull]
        private readonly XElement _element;

        [CanBeNull]
        private string _value;

        public XmlInnerTextNode([NotNull] XmlTextNode textNode, [NotNull] XElement element)
        {
            ParentNode = textNode;
            _element = element;
        }

        public IEnumerable<ITextNode> Attributes => Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes => Enumerable.Empty<ITextNode>();

        public string Key { get; } = string.Empty;

        public ITextNode ParentNode { get; }

        public TextSpan TextSpan => ParentNode?.TextSpan ?? TextSpan.Empty;

        public ISnapshot Snapshot => ParentNode?.Snapshot ?? Snapshots.Snapshot.Empty;

        public string Value => _value ?? (_value = RemoveNamespaces.Replace(string.Join(string.Empty, _element.Nodes().Select(n => n.ToString(SaveOptions.OmitDuplicateNamespaces)).ToArray()).Trim(), string.Empty));

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

        public ITextNode GetFormatSpecificChildNode(string name)
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
            _element.Value = newValue;

            return true;
        }
    }
}
