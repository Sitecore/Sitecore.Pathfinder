// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Documents
{
    [DebuggerDisplay("\\{{GetType().FullName,nq}\\}: {Name,nq} = {Value}")]
    public class TextNode : ITextNode
    {
        public static readonly ITextNode Empty = new SnapshotTextNode(Documents.Snapshot.Empty);

        public TextNode([NotNull] ISnapshot snapshot, TextPosition position, [NotNull] string name, [NotNull] string value, [CanBeNull] ITextNode parent)
        {
            Snapshot = snapshot;
            Position = position;
            Name = name;
            Value = value;
            Parent = parent;
        }

        public IEnumerable<ITextNode> Attributes { get; } = new List<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = new List<ITextNode>();

        public string Name { get; }

        public ITextNode Parent { get; }

        public TextPosition Position { get; }

        public ISnapshot Snapshot { get; }

        public string Value { get; protected set; }

        public virtual string GetAttributeValue(string attributeName, string defaultValue = "")
        {
            var value = GetAttributeTextNode(attributeName)?.Value;
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        public virtual bool SetName(string newName)
        {
            return false;
        }

        [NotNull]
        public Attribute<T> GetAttribute<T>(string attributeName, SourceFlags sourceFlags = SourceFlags.None)
        {
            var attributeTextNode = GetAttributeTextNode(attributeName);

            Attribute<T> attribute;

            if (attributeTextNode != null)
            {
                attribute = new Attribute<T>(attributeTextNode, sourceFlags);
            }
            else if (typeof(T) == typeof(string))
            {
                // hacky, hacky, hacky - but to set the default of value to not null
                attribute = (Attribute<T>)((object)new Attribute<string>(attributeName, string.Empty));
            }
            else
            {
              return new Attribute<T>(attributeName, default(T));
            }

            attribute.SourceFlags = sourceFlags;
            return attribute;
        }

        public virtual ITextNode GetAttributeTextNode(string attributeName)
        {
            return Attributes.FirstOrDefault(a => a.Name == attributeName);
        }

        public virtual bool SetValue(string value)
        {
            return false;
        }
    }
}
