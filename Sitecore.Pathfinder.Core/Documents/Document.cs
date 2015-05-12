namespace Sitecore.Pathfinder.Documents
{
  using System;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects;

  public class Document : IDocument
  {
    public static readonly IDocument Empty = new Document(Projects.SourceFile.Empty);

    public Document([NotNull] ISourceFile sourceFile)
    {
      this.SourceFile = sourceFile;
      this.Root = new TreeNode(this, string.Empty);
    }

    public bool IsEditing { get; protected set; }

    public virtual ITreeNode Root { get; }

    public ISourceFile SourceFile { get; }

    public virtual void BeginEdit()
    {
      throw new InvalidOperationException("Document is not editable");
    }

    public virtual void EndEdit()
    {
      throw new InvalidOperationException("Document is not editable");
    }

    public void EnsureIsEditing()
    {
      if (!this.IsEditing)
      {
        throw new InvalidOperationException("Document is not in edit mode");
      }
    }
  }
}