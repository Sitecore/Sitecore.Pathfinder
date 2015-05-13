namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Security.Cryptography;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects.References;

  public abstract class ProjectItem : IProjectItem
  {
    private Guid guid = Guid.Empty;

    private string projectId;

    protected ProjectItem([NotNull] IProject project, [NotNull] ITreeNode treeNode)
    {
      this.Project = project;
      this.TreeNode = treeNode;
      this.References = new ReferenceCollection(this);

      this.ProjectId = PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.ProjectDirectory, treeNode.Document.SourceFile.SourceFileName));
    }

    public Guid Guid
    {
      get
      {
        if (this.guid == Guid.Empty)
        {
          // project id is a guid, keep it otherwise calculate it
          Guid g;
          if (!Guid.TryParse(this.ProjectId, out g))
          {
            // calculate guid from project unique id and project id
            var text = this.Project.ProjectUniqueId + "/" + this.ProjectId;
            var bytes = Encoding.UTF8.GetBytes(text);
            var hash = MD5.Create().ComputeHash(bytes);
            g = new Guid(hash);
          }

          this.guid = g;
        }

        return this.guid;
      }
    }

    public bool IsBindComplete { get; set; }

    [CanBeNull]
    public ProjectItem Owner { get; set; }

    [NotNull]
    public IProject Project { get; }

    [NotNull]
    public string ProjectId
    {
      get
      {
        return this.projectId;
      }

      set
      {
        this.projectId = value;
        this.guid = Guid.Empty;
      }
    }

    [NotNull]
    public abstract string QualifiedName { get; }

    [NotNull]
    public ReferenceCollection References { get; }

    [NotNull]
    public abstract string ShortName { get; }

    [NotNull]
    public ITreeNode TreeNode { get; }

    public abstract void Bind();

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