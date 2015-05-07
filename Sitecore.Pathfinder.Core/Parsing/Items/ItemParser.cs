namespace Sitecore.Pathfinder.Parsing.Items
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using Sitecore.Pathfinder.Diagnostics;

  [Export(typeof(IParser))]
  public class ItemParser : ParserBase
  {
    [ImportingConstructor]
    public ItemParser([NotNull] ICompositionService compositionService) : base(Items)
    {
      this.CompositionService = compositionService;
    }

    [NotNull]
    public ICompositionService CompositionService { get; }

    [NotNull]
    [ImportMany]
    public IEnumerable<IItemParser> ItemFileBuilders { get; private set; }

    public override void Parse(IParseContext context)
    {
      this.ParseItems(context, context.Project.ProjectDirectory);
    }

    protected virtual ParseResult ParseItem([NotNull] IParseContext context, [NotNull] string databaseName, [NotNull] string fileName)
    {
      // todo: use abstract factory pattern
      var itemFileBuildContext = new ItemParseContext(context, databaseName, fileName);
      var buildResult = ParseResult.None;

      foreach (var itemFileBuilder in this.ItemFileBuilders.OrderBy(b => b.Priority))
      {
        if (!itemFileBuilder.CanParse(itemFileBuildContext))
        {
          continue;
        }

        itemFileBuilder.Parse(itemFileBuildContext);
      }

      return buildResult;
    }

    protected virtual void ParseItems([NotNull] IParseContext context, [NotNull] string projectDirectory)
    {
      var serializationDirectory = Path.Combine(projectDirectory, "serialization");
      if (!context.FileSystem.DirectoryExists(serializationDirectory))
      {
        return;
      }

      foreach (var databaseDirectory in context.FileSystem.GetDirectories(serializationDirectory))
      {
        var databaseName = Path.GetFileNameWithoutExtension(databaseDirectory) ?? "master";

        this.ParseItems(context, databaseName, databaseDirectory);
      }
    }

    protected virtual void ParseItems([NotNull] IParseContext context, [NotNull] string databaseName, [NotNull] string directory)
    {
      var fileNames = this.GetFileNames(context, directory);

      foreach (var fileName in fileNames)
      {
        var result = this.ParseItem(context, databaseName, fileName);
        if (result != ParseResult.Success)
        {
          continue;
        }

        var subFileNames = this.GetFileNames(context, directory, fileName);
        foreach (var subFileName in subFileNames)
        {
          this.ParseItem(context, databaseName, subFileName);
        }
      }

      foreach (var subdirectory in context.FileSystem.GetDirectories(directory))
      {
        this.ParseItems(context, databaseName, subdirectory);
      }
    }

    [NotNull]
    private IEnumerable<string> GetFileNames([NotNull] IParseContext context, [NotNull] string directory, [NotNull] string parentFileName = "")
    {
      var pattern = string.IsNullOrEmpty(parentFileName) ? "*" : Path.GetFileName(parentFileName) + ".*";

      var fileNames = context.FileSystem.GetFiles(directory, pattern).ToList();

      fileNames.Remove(parentFileName);

      // remove sub file names
      var subFileNames = new List<string>();

      foreach (var fileName in fileNames)
      {
        var subFileName = fileName + ".";
        subFileNames.AddRange(fileNames.Where(f => f.StartsWith(subFileName, StringComparison.OrdinalIgnoreCase)));
      }

      fileNames.RemoveAll(f => subFileNames.Contains(f));

      return fileNames;
    }
  }
}
