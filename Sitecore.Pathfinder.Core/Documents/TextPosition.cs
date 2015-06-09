// © 2015 Sitecore Corporation A/S. All rights reserved.

namespace Sitecore.Pathfinder.Documents
{
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
