namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.TextDocuments;

  public class ProjectMessage
  {
    public ProjectMessage([NotNull] string fileName, TextPosition position, [NotNull] MessageType messageType, [NotNull] string text)
    {
      this.FileName = fileName;
      this.Position = position;
      this.MessageType = messageType;
      this.Text = text;
    }

    [NotNull]
    public string FileName { get; }

    public TextPosition Position { get; }

    [NotNull]
    public string Text { get; }

    [NotNull]
    public MessageType MessageType { get; }
  }
}
