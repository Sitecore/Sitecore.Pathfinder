namespace Sitecore.Pathfinder.Projects.Other
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Templates;

  public class Component : File
  {
    public Component([NotNull] IProject project, [NotNull] ITreeNode treeNode, [NotNull] Template privateTemplate, [NotNull] Template publicTemplate) : base(project, treeNode)
    {
      this.PrivateTemplate = privateTemplate;
      this.PublicTemplate = publicTemplate;

      Debug.Assert(this.PrivateTemplate.Owner != null, "Owner is already set");
      this.PrivateTemplate.Owner = this;

      Debug.Assert(this.PublicTemplate.Owner != null, "Owner is already set");
      this.PublicTemplate.Owner = this;
    }

    [NotNull]
    public Template PrivateTemplate { get; }

    [NotNull]
    public Template PublicTemplate { get; }
  }
}
