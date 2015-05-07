namespace Sitecore.Pathfinder.Parsing.Items
{
  using Sitecore.Pathfinder.Diagnostics;

  public interface IItemParseContext
  {
    [NotNull]
    string DatabaseName { get; set; }

    [NotNull]
    string FileName { get; }

    [NotNull]
    string ItemName { get; }

    [NotNull]
    string ItemPath { get; set; }

    [NotNull]
    string ParentItemPath { get; }

    [NotNull]
    IParseContext ParseContext { get; }
  }
}
