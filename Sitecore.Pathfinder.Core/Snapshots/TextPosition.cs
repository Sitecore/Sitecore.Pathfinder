// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("{GetType().Name,nq}: ({LineNumber,nq}, {LinePosition,nq}, {LineLength,nq}, )")]
    public struct TextPosition
    {
        public static readonly TextPosition Empty = new TextPosition(0, 0, 0);

        public TextPosition(int lineNumber, int linePosition, int lineLength)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            LineLength = lineLength;
        }

        public int LineLength { get; }

        public int LineNumber { get; }

        public int LinePosition { get; }

        public bool Equals(TextPosition other)
        {
            return LineLength == other.LineLength && LineNumber == other.LineNumber && LinePosition == other.LinePosition;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TextPosition && Equals((TextPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = LineLength;
                hashCode = (hashCode * 397) ^ LineNumber;
                hashCode = (hashCode * 397) ^ LinePosition;
                return hashCode;
            }
        }

        public static bool operator ==(TextPosition c1, TextPosition c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(TextPosition c1, TextPosition c2)
        {
            return !c1.Equals(c2);
        }
    }
}
