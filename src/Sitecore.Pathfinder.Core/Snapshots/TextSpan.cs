// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;
using System.Linq;
using System.Xml;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

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

        public int Length { get; }

        public int LineNumber { get; }

        public int LinePosition { get; }

        public bool Equals(TextSpan other)
        {
            return Length == other.Length && LineNumber == other.LineNumber && LinePosition == other.LinePosition;
        }

        public override string ToString()
        {
            return Length == 0 ? $"({LineNumber},{LinePosition})" : $"({LineNumber},{LinePosition},{LineNumber},{LinePosition + Length})";
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

        public static bool TryParse([NotNull] string text, out TextSpan span)
        {
            span = Empty;
            if (!text.StartsWith("(") || !text.EndsWith(")"))
            {
                return false;
            }

            var parts = text.Mid(1, text.Length - 2).Split(',').Select(s => s.Trim()).ToArray();
            if (parts.Length != 2 && parts.Length != 4)
            {
                return false;
            }

            int lineNumber;
            if (!int.TryParse(parts[0], out lineNumber))
            {
                return false;
            }

            int linePosition;
            if (!int.TryParse(parts[1], out linePosition))
            {
                return false;
            }

            var length = 0;
            if (parts.Length == 4)
            {
                if (!int.TryParse(parts[3], out length))
                {
                    return false;
                }

                length -= linePosition;
            }

            span = new TextSpan(lineNumber, linePosition, length);
            return true;
        }
    }
}
