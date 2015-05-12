namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Projects;

  public class TextSpan : ITextSpan
  {
    public TextSpan(ISourceFile sourceFile, int lineNumber = 0, int linePosition = 0)
    {
      this.SourceFile = sourceFile;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public ISourceFile SourceFile { get; }
  }
}