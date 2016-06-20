// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Snapshots
{
    public class FileNameTextNode : ITextNode, IMutableTextNode
    {
        public FileNameTextNode([NotNull] string itemName, [NotNull] ISnapshot snapshot)
        {
            Value = itemName;
            Snapshot = snapshot;
        }

        public IEnumerable<ITextNode> Attributes { get; } = Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = Enumerable.Empty<ITextNode>();

        public string Key => Value;

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan { get; } = TextSpan.Empty;

        public string Value { get; }

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
            return ((IMutableTextNode)this).SetValue(newKey);
        }

        bool IMutableTextNode.SetValue(string newValue)
        {
            var fileName = Snapshot.SourceFile.AbsoluteFileName;
            var extension = PathHelper.GetExtension(Snapshot.SourceFile.AbsoluteFileName);

            var newFileName = Path.Combine(Path.GetDirectoryName(fileName) ?? string.Empty, newValue + extension);

            // todo: use FileSystemService
            // File.Move(fileName, newFileName);

            // todo: update Project Unique ID
            return true;
        }
    }
}
