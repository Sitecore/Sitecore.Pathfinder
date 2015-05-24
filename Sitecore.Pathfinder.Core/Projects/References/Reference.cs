namespace Sitecore.Pathfinder.Projects.References
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  [DebuggerDisplay("{GetType().Name,nq}: {TargetQualifiedName}")]
  public class Reference : IReference
  {
    private bool isValid;

    private Guid targetProjectItemGuid = Guid.Empty;

    public Reference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName)
    {
      this.Owner = owner;
      this.TargetQualifiedName = targetQualifiedName;
    }

    public Reference([NotNull] IProjectItem owner, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName)
    {
      this.Owner = owner;
      this.SourceTextNode = sourceTextNode;
      this.TargetQualifiedName = targetQualifiedName;
    }

    public bool IsResolved { get; set; }

    public bool IsValid
    {
      get
      {
        if (!this.IsResolved)
        {
          this.Resolve();
        }

        return this.isValid;
      }

      protected set
      {
        this.isValid = value;
      }
    }

    public IProjectItem Owner { get; }

    public ITextNode SourceTextNode { get; set; }

    public string TargetQualifiedName { get; }

    public void Invalidate()
    {
      this.IsResolved = false;
      this.IsValid = false;
      this.targetProjectItemGuid = Guid.Empty;
    }

    public virtual IProjectItem Resolve()
    {
      if (this.IsResolved)
      {
        if (!this.IsValid)
        {
          return null;
        }

        var result = this.Owner.Project.Items.FirstOrDefault(i => i.Guid == this.targetProjectItemGuid);
        if (result == null)
        {
          this.IsValid = false;
          this.targetProjectItemGuid = Guid.Empty;
        }

        return result;
      }

      this.IsResolved = true;

      var projectItem = this.Owner.Project.Items.FirstOrDefault(i => string.Compare(i.QualifiedName, this.TargetQualifiedName, StringComparison.OrdinalIgnoreCase) == 0);
      if (projectItem == null)
      {
        this.IsValid = false;
        this.targetProjectItemGuid = Guid.Empty;
        return null;
      }

      this.targetProjectItemGuid = projectItem.Guid;
      this.IsValid = true;

      return projectItem;
    }
  }
}
