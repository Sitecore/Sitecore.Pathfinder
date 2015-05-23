namespace Sitecore.Pathfinder.TextDocuments
{
  public struct TextPosition
  {
    public static readonly TextPosition Empty = new TextPosition(0, 0, 0);

    public TextPosition(int lineNumber, int linePosition, int lineLength)
    {
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
      this.LineLength = lineLength;
    }

    public int LineLength { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }
  }
}
