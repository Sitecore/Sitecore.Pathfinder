namespace Sitecore.Pathfinder.Projects.Other
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;

  public class Component : FileBase
  {
    public Component([NotNull] ISourceFile sourceFileName, [NotNull] Item privateTemplate, [NotNull] Item publicTemplate) : base(sourceFileName)
    {
      this.PrivateTemplate = privateTemplate;
      this.PublicTemplate = publicTemplate;
    }

    [NotNull]
    public Item PrivateTemplate { get; }

    [NotNull]
    public Item PublicTemplate { get; }
  }
}
