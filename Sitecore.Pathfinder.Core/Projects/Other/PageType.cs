namespace Sitecore.Pathfinder.Projects.Other
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;

  public class PageType : File
  {
    public PageType(IProject project, [NotNull] ISourceFile sourceFileName) : base(project, sourceFileName)
    {
    }

    [NotNull]
    public ICollection<Item> Components { get; } = new List<Item>();
  }
}
