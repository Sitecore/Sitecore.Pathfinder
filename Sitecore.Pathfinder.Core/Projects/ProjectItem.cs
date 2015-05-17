namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Security.Cryptography;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.References;
  using Sitecore.Pathfinder.TextDocuments;

  public abstract class ProjectItem : IProjectItem
  {
    protected ProjectItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] IDocument document)
    {
      this.Project = project;
      this.Document = document;
      this.References = new ReferenceCollection(this);

      this.OverwriteProjectUniqueId(projectUniqueId);
    }

    public Guid Guid { get; private set; }

    public IProjectItem Owner { get; set; }

    public IProject Project { get; }

    public string ProjectUniqueId { get; private set; }

    public abstract string QualifiedName { get; }

    public ReferenceCollection References { get; }

    public abstract string ShortName { get; }

    public IDocument Document { get; }

    public abstract void Bind();

    public virtual void Lint()
    {
      foreach (var reference in this.References)
      {
        if (!reference.Resolve())
        {
          this.Project.Trace.TraceWarning(Texts.Text3024, this.Document.SourceFile.SourceFileName, 0, 0, reference.ToString());
        }
      }
    }

    protected internal void OverwriteProjectUniqueId([NotNull] string newProjectUniqueId)
    {
      this.ProjectUniqueId = newProjectUniqueId;
      this.SetGuid();
    }

    private void SetGuid()
    {
      Guid guid;
      if (!Guid.TryParse(this.ProjectUniqueId, out guid))
      {
        // calculate guid from project unique id and project id
        var text = this.Project.ProjectUniqueId + "/" + this.ProjectUniqueId;
        var bytes = Encoding.UTF8.GetBytes(text);
        var hash = MD5.Create().ComputeHash(bytes);
        guid = new Guid(hash);
      }

      this.Guid = guid;
    }
  }
}