namespace Sitecore.Pathfinder.TreeNodes
{
  using System.Xml;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public class TextSpan : ITextSpan
  {
    public static readonly ITextSpan Empty = new TextSpan(TreeNodes.Document.Empty);

    public TextSpan([NotNull] IDocument document, int lineNumber = 0, int linePosition = 0)
    {
      this.Document = document;
      this.LineNumber = lineNumber;
      this.LinePosition = linePosition;
    }

    public IDocument Document { get; }

    public int LineNumber { get; }

    public int LinePosition { get; }
  }
}
