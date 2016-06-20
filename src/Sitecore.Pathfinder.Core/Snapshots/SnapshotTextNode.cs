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

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan { get; } = TextSpan.Empty;

        public string Value => Snapshot.SourceFile.ProjectFileName;

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
    }
}
