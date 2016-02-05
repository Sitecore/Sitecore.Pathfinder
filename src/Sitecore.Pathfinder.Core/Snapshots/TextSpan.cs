// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("{GetType().Name,nq}: ({LineNumber,nq}, {LinePosition,nq}, {Length,nq})")]
    public struct TextSpan
    {
        public static readonly TextSpan Empty = new TextSpan(0, 0, 0);

        public TextSpan(int lineNumber, int linePosition, int length)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            Length = length;
        }

        public TextSpan([NotNull] IXmlLineInfo lineInfo)
        {
            LineNumber = lineInfo.LineNumber;
            LinePosition = lineInfo.LinePosition;
            Length = 0;
        }

        public int Length { get; }

        public int LineNumber { get; }

        public int LinePosition { get; }

        public bool Equals(TextSpan other)
        {
            return Length == other.Length && LineNumber == other.LineNumber && LinePosition == other.LinePosition;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TextSpan && Equals((TextSpan)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Length;
                hashCode = (hashCode * 397) ^ LineNumber;
                hashCode = (hashCode * 397) ^ LinePosition;
                return hashCode;
            }
        }

        public static bool operator ==(TextSpan c1, TextSpan c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(TextSpan c1, TextSpan c2)
        {
            return !c1.Equals(c2);
        }
    }
}
