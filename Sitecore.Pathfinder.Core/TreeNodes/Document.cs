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
    }

    public virtual ITreeNode Root { get; protected set; } = TreeNode.Empty;

    public ISourceFile SourceFile { get; }
  }
}
