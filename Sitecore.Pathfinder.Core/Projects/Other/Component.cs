namespace Sitecore.Pathfinder.Projects.Other
{
  using System.Diagnostics;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public class Component : FileBase
  {
    public Component([NotNull] ISourceFile sourceFileName, [NotNull] Item privateTemplate, [NotNull] Item publicTemplate) : base(sourceFileName)
    {
      this.PrivateTemplate = privateTemplate;
      this.PublicTemplate = publicTemplate;

      Debug.Assert(this.PrivateTemplate.Owner != null, "Owner is already set");
      this.PrivateTemplate.Owner = this;

      Debug.Assert(this.PublicTemplate.Owner != null, "Owner is already set");
      this.PublicTemplate.Owner = this;
    }

    [NotNull]
    public Item PrivateTemplate { get; }

    [NotNull]
    public Item PublicTemplate { get; }
  }
}
