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

        public string Key { get; } = string.Empty;

        public ITextNode ParentNode { get; } = null;

        public TextSpan TextSpan { get; } = TextSpan.Empty;

        public ISnapshot Snapshot { get; }

        public string Value => Snapshot.SourceFile.ProjectFileName;

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

        public ITextNode GetLogicalChildNode(string name)
        {
            return null;
        }

        public bool SetKey(string newKey)
        {
            return false;
        }

        public bool SetValue(string newValue)
        {
            return false;
        }
    }
}
