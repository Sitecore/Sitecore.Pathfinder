namespace Sitecore.Pathfinder.Projects
{
  using System;
  using System.Xml.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;

  public class SourceFile : ISourceFile
  {
    public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string sourceFileName)
    {
      this.FileSystem = fileSystem;

      this.SourceFileName = sourceFileName;
      this.LastWriteTimeUtc = fileSystem.GetLastWriteTimeUtc(this.SourceFileName);
    }

    public bool IsModified { get; set; }

    public DateTime LastWriteTimeUtc { get; private set; }

    public string SourceFileName { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    public string[] ReadAsLines()
    {
      return this.FileSystem.ReadAllLines(this.SourceFileName);
    }

    public XElement ReadAsXml()
    {
      var text = this.FileSystem.ReadAllText(this.SourceFileName);

      XDocument doc;
      try
      {
        doc = XDocument.Parse(text, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
      }
      catch
      {
        throw new BuildException(Texts.Text1000, this.SourceFileName);
      }

      var root = doc.Root;
      if (root == null)
      {
        throw new BuildException(Texts.Text1000, this.SourceFileName);
      }

      return root;
    }

    public void Touch()
    {
      this.LastWriteTimeUtc = DateTime.MinValue;
    }
  }
}
