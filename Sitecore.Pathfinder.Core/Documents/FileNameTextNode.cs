// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Documents
{
    public class FileNameTextNode : ITextNode
    {
        public FileNameTextNode([NotNull] string itemName, [NotNull] ISnapshot snapshot)
        {
            Value = itemName;
            Snapshot = snapshot;
        }

        public IEnumerable<ITextNode> Attributes { get; } = new TextNode[0];

        public IEnumerable<ITextNode> ChildNodes { get; } = new TextNode[0];

        public string Name { get; } = string.Empty;

        public ITextNode Parent { get; } = null;

        public TextPosition Position { get; } = TextPosition.Empty;

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
