namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

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

    public bool IsModified { get; set; }

    public DateTime LastWriteTimeUtc { get; }

    public string SourceFileName { get; }

    public string SourceFileNameWithoutExtensions { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    public JObject ReadAsJson()
    {
      try
      {
        var contents = this.ReadAsText();
        return JObject.Parse(contents);
      }
      catch (Exception ex)
      {
        throw new BuildException(Texts.Text2000, this, ex.Message);
      }
    }

    public string[] ReadAsLines()
    {
      return this.FileSystem.ReadAllLines(this.SourceFileName);
    }

    public string ReadAsText()
    {
      var contents = this.FileSystem.ReadAllText(this.SourceFileName);
      return contents;
    }

    public XElement ReadAsXml()
    {
      var contents = this.ReadAsText();

      XDocument doc;
      try
      {
        doc = XDocument.Parse(contents, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
      }
      catch (Exception ex)
      {
        throw new BuildException(Texts.Text2000, this, ex.Message);
      }

      var root = doc.Root;
      if (root == null)
      {
        throw new BuildException(Texts.Text2000, this);
      }

      return root;
    }
  }
}
