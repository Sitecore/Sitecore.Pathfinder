// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots.Xml
{
    public class XmlInnerTextNode : ITextNode
    {
        private static readonly Regex RemoveNamespaces = new Regex("\\sxmlns[^\"]+\"[^\"]+\"", RegexOptions.Compiled);

        private readonly XElement _element;

        private string _value;

        public XmlInnerTextNode([NotNull] XmlTextNode textNode, [NotNull] XElement element)
        {
            Parent = textNode;
            _element = element;
        }

        public IEnumerable<ITextNode> Attributes => Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes => Enumerable.Empty<ITextNode>();

        public string Name { get; } = string.Empty;

        public ITextNode Parent { get; }

        public TextSpan Span => Parent?.Span ?? TextSpan.Empty;

        public ISnapshot Snapshot => Parent?.Snapshot ?? Snapshots.Snapshot.Empty;

        public string Value => _value ?? (_value = RemoveNamespaces.Replace(string.Join(string.Empty, _element.Nodes().Select(n => n.ToString(SaveOptions.OmitDuplicateNamespaces)).ToArray()).Trim(), string.Empty));

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
            _element.Value = value;

            return true;
        }
    }
}
