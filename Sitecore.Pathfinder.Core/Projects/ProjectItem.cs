namespace Sitecore.Pathfinder.Projects
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class ProjectItem
  {
    protected ProjectItem([NotNull] IProject project, [NotNull] ITreeNode treeNode)
    {
      this.Project = project;
      this.TreeNode = treeNode;
      this.References = new ReferenceCollection(this);
    }

    public bool IsAnalyzed { get; set; }

    [CanBeNull]
    public ProjectItem Owner { get; set; }

    [NotNull]
    public IProject Project { get; }

    [NotNull]
    public abstract string QualifiedName { get; }

    [NotNull]
    public ReferenceCollection References { get; }

    [NotNull]
    public abstract string ShortName { get; }

    [NotNull]
    public ITreeNode TreeNode { get; }

    public abstract void Analyze();

    public virtual void Lint()
    {
      foreach (var reference in this.References)
      {
        if (!reference.Resolve())
        {
          this.Project.Trace.TraceWarning(Texts.Text3024, this.TreeNode.Document.SourceFile.SourceFileName, 0, 0, reference.ToString());
        }
      }
    }
  }
}