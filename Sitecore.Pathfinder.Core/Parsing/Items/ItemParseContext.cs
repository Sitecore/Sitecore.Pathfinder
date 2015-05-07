namespace Sitecore.Pathfinder.Parsing.Items
{
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;

  public class ItemParseContext : IItemParseContext
  {
    public ItemParseContext([NotNull] IParseContext parseContext, [NotNull] string databaseName, [NotNull] string fileName)
    {
      this.ParseContext = parseContext;
      this.DatabaseName = databaseName;
      this.FileName = fileName;

      this.ItemPath = this.GetItemPath(parseContext, this.FileName);

      parseContext.CompositionService.SatisfyImportsOnce(this);
    }

    public string DatabaseName { get; set; }

    public string FileName { get; }

    [NotNull]
    public string ItemName
    {
      get
      {
        var n = this.ItemPath.LastIndexOf('/');
        return n >= 0 ? this.ItemPath.Mid(n + 1) : this.ItemPath;
      }
    }

    public string ItemPath { get; set; }

    public string ParentItemPath
    {
      get
      {
        var n = this.ItemPath.LastIndexOf('/');
        return this.ItemPath.Left(n);
      }
    }

    public IParseContext ParseContext { get; }

    [NotNull]
    protected string GetFileType([NotNull] string parentFileName, [NotNull] string fileName)
    {
      var n = fileName.LastIndexOf('\\');
      if (n < 0)
      {
        n = 0;
      }

      n = fileName.IndexOf('.', n);

      return n < 0 ? string.Empty : fileName.Mid(n);
    }

    [NotNull]
    protected string GetItemPath([NotNull] IParseContext parseContext, [NotNull] string fileName)
    {
      // cut off project directory
      var itemPath = fileName.Mid(parseContext.Project.ProjectDirectory.Length).Replace("\\", "/").TrimStart('/');

      // cut off serialization directory
      var n = itemPath.IndexOf('/');
      itemPath = itemPath.Mid(n).TrimStart('/');

      // cut database name
      n = itemPath.IndexOf('/');
      itemPath = itemPath.Mid(n);

      n = itemPath.LastIndexOf('/');
      if (n < 0)
      {
        return itemPath;
      }

      n = itemPath.IndexOf('.', n);

      if (n >= 0)
      {
        itemPath = itemPath.Left(n);
      }

      return itemPath;
    }
  }
}
