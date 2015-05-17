namespace Sitecore.Pathfinder.Projects.Other
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Files;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.TextDocuments;

  public class PageType : File
  {
    public PageType([NotNull] IProject project, [NotNull] IDocument document) : base(project, document)
    {
    }

    [NotNull]
    public ICollection<Item> Components { get; } = new List<Item>();
  }
}
