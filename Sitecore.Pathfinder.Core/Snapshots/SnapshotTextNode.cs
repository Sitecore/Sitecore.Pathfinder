// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public class SnapshotTextNode : ITextNode
    {
        public SnapshotTextNode([NotNull] ISnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public IEnumerable<ITextNode> Attributes => Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes => Enumerable.Empty<ITextNode>();

        public string Name { get; } = string.Empty;

        public ITextNode Parent { get; } = null;

        public TextPosition Position { get; } = TextPosition.Empty;

        public ISnapshot Snapshot { get; }

        public string Value => Snapshot.SourceFile.GetFileNameWithoutExtensions();

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
            return false;
        }
    }
}
