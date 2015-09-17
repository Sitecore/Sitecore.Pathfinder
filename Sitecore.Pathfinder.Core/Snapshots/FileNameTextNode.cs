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

        public string Name => Value;

        public ITextNode Parent { get; } = null;

        public TextSpan Span { get; } = TextSpan.Empty;

        public ISnapshot Snapshot { get; }

        public string Value { get; }

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
            return SetValue(newName);
        }

        public bool SetValue(string value)
        {
            var fileName = Snapshot.SourceFile.FileName;
            var extension = PathHelper.GetExtension(Snapshot.SourceFile.FileName);

            var newFileName = Path.Combine(Path.GetDirectoryName(fileName) ?? string.Empty, value + extension);

            // todo: use FileSystemService
            File.Move(fileName, newFileName);

            // todo: update Project Unique ID
            return true;
        }
    }
}
