namespace Sitecore.Pathfinder.Building.Builders.ItemFiles
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using Sitecore.Extensions.StringExtensions;
  using Sitecore.Pathfinder.Data.FieldHandlers;

  public class ItemFileBuildContext : IItemFileBuildContext
  {
    public ItemFileBuildContext([NotNull] IEmitContext buildContext, [NotNull] string databaseName, [NotNull] string fileName)
    {
      this.BuildContext = buildContext;
      this.DatabaseName = databaseName;
      this.FileName = fileName;

      this.ItemPath = this.GetItemPath(buildContext, this.FileName);

      buildContext.CompositionService.SatisfyImportsOnce(this);
    }

    public IEmitContext BuildContext { get; }

    public string DatabaseName { get; }

    [ImportMany]
    public IEnumerable<IFieldHandler> FieldHandlers { get; protected set; }

    public string FileName { get; }

    public string ItemPath { get; }

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
    protected string GetItemPath([NotNull] IBuildContextBase buildContext, [NotNull] string fileName)
    {
      // cut off project directory
      var itemPath = fileName.Mid(buildContext.ProjectDirectory.Length).Replace("\\", "/").TrimStart('/');

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
