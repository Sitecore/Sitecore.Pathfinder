// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public class StringTextNode : ITextNode, IMutableTextNode
    {
        private bool _hasTextSpan;

        private TextSpan _textSpan = TextSpan.Empty;

        public StringTextNode([NotNull] string text, [NotNull] ISnapshot snapshot)
        {
            Value = text;
            Snapshot = snapshot;
        }

        public IEnumerable<ITextNode> Attributes { get; } = Enumerable.Empty<ITextNode>();

        public IEnumerable<ITextNode> ChildNodes { get; } = Enumerable.Empty<ITextNode>();

        public string Key => Value;

        public ISnapshot Snapshot { get; }

        public TextSpan TextSpan
        {
            get
            {
                if (_hasTextSpan)
                {
                    return _textSpan;
                }

                _hasTextSpan = true;

                var lines = Snapshot.SourceFile.ReadAsLines();

                for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                {
                    var line = lines[lineNumber];

                    var linePosition = line.IndexOf(Value, StringComparison.Ordinal);
                    if (linePosition >= 0)
                    {
                        _textSpan = new TextSpan(lineNumber + 1, linePosition + 1, Value.Length);
                        break;
                    }

                    // try without backslashes as it may have been unescaped
                    if (line.IndexOf('\\') >= 0)
                    {
                        linePosition = line.Replace("\\", string.Empty).IndexOf(Value, StringComparison.Ordinal);
                        if (linePosition >= 0)
                        {
                            _textSpan = new TextSpan(lineNumber + 1, linePosition + 1, Value.Length);
                            break;
                        }
                    }
                }

                return _textSpan;
            }
        }

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

        public ITextNode GetInnerTextNode()
        {
            return null;
        }

        public ITextNode GetSnapshotLanguageSpecificChildNode(string name)
        {
            return null;
        }

        public bool HasAttribute(string attributeName)
        {
            return false;
        }

        bool IMutableTextNode.SetKey(string newKey)
        {
            return ((IMutableTextNode)this).SetValue(newKey);
        }

        bool IMutableTextNode.SetValue(string newValue)
        {
            return false;
        }
    }
}
