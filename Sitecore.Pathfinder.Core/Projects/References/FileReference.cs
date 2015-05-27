namespace Sitecore.Pathfinder.Projects.References
{
  using System;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Files;

  public class FileReference : Reference
  {
    public FileReference([NotNull] IProjectItem owner, [NotNull] string targetQualifiedName) : base(owner, targetQualifiedName)
    {
    }

    public FileReference([NotNull] IProjectItem owner, [NotNull] ITextNode sourceTextNode, [NotNull] string targetQualifiedName) : base(owner, sourceTextNode, targetQualifiedName)
    {
    }

    public override IProjectItem Resolve()
    {
      if (this.IsResolved)
      {
        if (!this.IsValid)
        {
          return null;
        }

        var result = this.Owner.Project.Items.FirstOrDefault(i => i.Guid == this.TargetProjectItemGuid);
        if (result == null)
        {
          this.IsValid = false;
          this.TargetProjectItemGuid = Guid.Empty;
        }

        return result;
      }

      this.IsResolved = true;

      var projectItem = this.Owner.Project.Items.OfType<File>().FirstOrDefault(i => string.Compare(i.FilePath, this.TargetQualifiedName, StringComparison.OrdinalIgnoreCase) == 0);
      if (projectItem == null)
      {
        this.IsValid = false;
        this.TargetProjectItemGuid = Guid.Empty;
        return null;
      }

      this.TargetProjectItemGuid = projectItem.Guid;
      this.IsValid = true;

      return projectItem;
    }
  }
}
