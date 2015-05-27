namespace Sitecore.Pathfinder.Documents
{
  using Sitecore.Pathfinder.Diagnostics;

  public class Snapshot : ISnapshot
  {
    public static readonly ISnapshot Empty = new Snapshot(Documents.SourceFile.Empty);

    public Snapshot([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
    }

    public ISourceFile SourceFile { get; }

    public virtual ITextNode GetNestedTextNode(ITextNode textNode, string name)
    {
      return null;
    }
  }
}
