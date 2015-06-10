// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Diagnostics;

namespace Sitecore.Pathfinder.Documents
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
    }
}
