namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.Projects;

  public class SourceFile : ISourceFile
  {
    public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string sourceFileName)
    {
      this.FileSystem = fileSystem;

      this.SourceFileName = sourceFileName;
      this.SourceFileNameWithoutExtensions = PathHelper.GetDirectoryAndFileNameWithoutExtensions(sourceFileName);
      this.LastWriteTimeUtc = fileSystem.GetLastWriteTimeUtc(this.SourceFileName);
    }

    [NotNull]
    public static ISourceFile Empty { get; } = new EmptySourceFile();

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    public bool IsModified { get; set; }

    public DateTime LastWriteTimeUtc { get; }

    public string SourceFileName { get; }

    public string SourceFileNameWithoutExtensions { get; }

    public string[] ReadAsLines(IParseContext context)
    {
      var lines = this.FileSystem.ReadAllLines(this.SourceFileName);

      for (var index = 0; index < lines.Length; index++)
      {
        lines[index] = context.ReplaceTokens(lines[index]);
      }

      return lines;
    }

    public string ReadAsText(IParseContext context)
    {
      var contents = this.FileSystem.ReadAllText(this.SourceFileName);
      contents = context.ReplaceTokens(contents);
      return contents;
    }

    public JObject ReadAsJson(IParseContext context)
    {
      var contents = this.ReadAsText(context);
      return JObject.Parse(contents);
    }

    public XElement ReadAsXml(IParseContext context)
    {
      var contents = this.ReadAsText(context);

      XDocument doc;
      try
      {
        doc = XDocument.Parse(contents, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
      }
      catch
      {
        throw new BuildException(Texts.Text2000, this.SourceFileName);
      }

      var root = doc.Root;
      if (root == null)
      {
        throw new BuildException(Texts.Text2000, this.SourceFileName);
      }

      return root;
    }
  }
}