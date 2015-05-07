namespace Sitecore.Pathfinder.Parsing.ContentFiles
{
  using System;
  using System.ComponentModel.Composition;
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Models.ContentFiles;

  [Export(typeof(IParser))]
  public class ContentFileParser : ParserBase
  {
    public ContentFileParser() : base(ContentFiles)
    {
    }

    public override void Parse(IParseContext context)
    {
      this.ParseFiles(context, context.Project.ProjectDirectory);
    }

    protected void ParseContentFile([NotNull] IParseContext context, [NotNull] string fileName, [NotNull] string destinationDirectory)
    {
      var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(fileName) ?? string.Empty);

      var contentFileModel = new ContentFileModel(fileName, destinationFileName);
      context.Project.Models.Add(contentFileModel);
    }

    protected virtual void ParseFiles([NotNull] IParseContext context, [NotNull] string projectDirectory)
    {
      var contentDirectory = Path.Combine(projectDirectory, "content");
      if (!context.FileSystem.DirectoryExists(contentDirectory))
      {
        return;
      }

      foreach (var fileName in context.FileSystem.GetFiles(contentDirectory))
      {
        this.ParseContentFile(context, fileName, string.Empty);
      }

      foreach (var rootDirectory in context.FileSystem.GetDirectories(contentDirectory))
      {
        var directoryName = Path.GetFileNameWithoutExtension(rootDirectory) ?? string.Empty;

        // skip /bin folder - it will be copied last by the BinFileParser class
        if (string.Compare(directoryName, "bin", StringComparison.OrdinalIgnoreCase) == 0)
        {
          continue;
        }

        var directory = Path.Combine(string.Empty, directoryName);
        this.ParseFiles(context, rootDirectory, directory);
      }
    }

    protected void ParseFiles([NotNull] IParseContext context, [NotNull] string projectDirectory, [NotNull] string destinationDirectory)
    {
      foreach (var fileName in context.FileSystem.GetFiles(projectDirectory))
      {
        this.ParseContentFile(context, fileName, destinationDirectory);
      }

      foreach (var subdirectory in context.FileSystem.GetDirectories(projectDirectory))
      {
        var dir = Path.Combine(destinationDirectory, Path.GetFileName(subdirectory) ?? string.Empty);
        this.ParseFiles(context, subdirectory, dir);
      }
    }
  }
}
