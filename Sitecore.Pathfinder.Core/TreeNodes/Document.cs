namespace Sitecore.Pathfinder.TreeNodes
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public class Document : IDocument
  {
    public static readonly IDocument Empty = new Document(Projects.SourceFile.Empty);

    public Document([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
      this.Root = new TreeNode(string.Empty, new TextSpan(this));
    }

    public virtual ITreeNode Root { get; }

    public ISourceFile SourceFile { get; }
  }
}
