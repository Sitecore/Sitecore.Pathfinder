// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public class FileNameTextNode : ITextNode
    {
        public FileNameTextNode([NotNull] string fileName, [NotNull] ISnapshot snapshot)
        {
            Value = fileName;
            Snapshot = snapshot;
        }

        public IEnumerable<ITextNode> Attributes { get; } = Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = Enumerable.Empty<ITextNode>();

        public string Key => Value;

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan { get; } = TextSpan.Empty;

        public string Value { get; }

        public ITextNode Inner => null;
    }
}
