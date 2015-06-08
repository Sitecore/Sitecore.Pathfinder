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
    private static readonly MD5 Md5Hash = MD5.Create();

    protected ProjectItem([NotNull] IProject project, [NotNull] string projectUniqueId, [NotNull] ISnapshot snapshot)
    {
      this.Project = project;
      this.ProjectUniqueId = projectUniqueId;
      this.Snapshot = snapshot;

      this.References = new ReferenceCollection(this);

      this.SetGuid();
    }

    public Guid Guid { get; private set; }

    public IProject Project { get; }

    public string ProjectUniqueId { get; private set; }

    public abstract string QualifiedName { get; }

    public ReferenceCollection References { get; }

    public abstract string ShortName { get; }

    public ISnapshot Snapshot { get; }

    public abstract void Rename(string newQualifiedName);

    protected virtual void Merge([NotNull] IProjectItem newProjectItem, bool overwrite)
    {
      if (!overwrite)
      {
        return;
      }

      this.ProjectUniqueId = newProjectItem.ProjectUniqueId;
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
        var hash = Md5Hash.ComputeHash(bytes);
        guid = new Guid(hash);
      }

      this.Guid = guid;
    }
  }
}
