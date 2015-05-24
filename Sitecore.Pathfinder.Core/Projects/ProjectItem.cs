namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Diagnostics;
  using System.Security.Cryptography;
  using System.Text;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.References;

  [DebuggerDisplay("{GetType().Name,nq}: {QualifiedName}")]
  public abstract class ProjectItem : IProjectItem
  {
    private static readonly MD5 MD5Hash = MD5.Create();

    protected ProjectItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] IDocumentSnapshot documentSnapshot)
    {
      this.Project = project;
      this.DocumentSnapshot = documentSnapshot;
      this.References = new ReferenceCollection(this);

      this.OverwriteProjectUniqueId(projectUniqueId);
    }

    // todo: !!!must be read only!!!
    public Guid Guid { get; set; }

    public IProjectItem Owner { get; set; }

    public IProject Project { get; }

    public string ProjectUniqueId { get; private set; }

    public abstract string QualifiedName { get; }

    public ReferenceCollection References { get; }

    public abstract string ShortName { get; }

    public IDocumentSnapshot DocumentSnapshot { get; }

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
        var hash = MD5Hash.ComputeHash(bytes);
        guid = new Guid(hash);
      }

      this.Guid = guid;
    }
  }
}