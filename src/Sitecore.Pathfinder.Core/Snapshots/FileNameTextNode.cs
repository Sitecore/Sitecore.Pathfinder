// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Snapshots
{
    public class FileNameTextNode : ITextNode
    {
        public FileNameTextNode([NotNull] string itemName, [NotNull] ISnapshot snapshot)
        {
            Value = itemName;
            Snapshot = snapshot;
        }

        public IEnumerable<ITextNode> Attributes { get; } = Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = Enumerable.Empty<ITextNode>();

        public string Key => Value;

        public ITextNode ParentNode { get; } = null;

        public TextSpan TextSpan { get; } = TextSpan.Empty;

        public ISnapshot Snapshot { get; }

        public string Value { get; }

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
            return SetValue(newKey);
        }

        public bool SetValue(string newValue)
        {
            var fileName = Snapshot.SourceFile.AbsoluteFileName;
            var extension = PathHelper.GetExtension(Snapshot.SourceFile.AbsoluteFileName);

            var newFileName = Path.Combine(Path.GetDirectoryName(fileName) ?? string.Empty, newValue + extension);

            // todo: use FileSystemService
            File.Move(fileName, newFileName);

            // todo: update Project Unique ID
            return true;
        }
    }
}
