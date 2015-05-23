namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class Diagnostic
  {
    public Diagnostic([NotNull] string fileName, TextPosition position, Severity severity, [NotNull] string text)
    {
      this.FileName = fileName;
      this.Position = position;
      this.Severity = severity;
      this.Text = text;
    }

    [NotNull]
    public string FileName { get; }

    public TextPosition Position { get; }

    [NotNull]
    public string Text { get; }

    [NotNull]
    public Severity Severity { get; }
  }
}
