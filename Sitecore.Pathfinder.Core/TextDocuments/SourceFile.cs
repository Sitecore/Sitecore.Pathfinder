namespace Sitecore.Pathfinder.TextDocuments
{
  using System;
  using System.Diagnostics;
  using System.Xml.Linq;
  using Newtonsoft.Json.Linq;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  [DebuggerDisplay("{GetType().Name}: FileName={FileName}")]
  public class SourceFile : ISourceFile
  {
    public SourceFile([NotNull] IFileSystemService fileSystem, [NotNull] string fileName)
    {
      this.FileSystem = fileSystem;
      this.FileName = fileName;

      this.LastWriteTimeUtc = this.FileSystem.GetLastWriteTimeUtc(this.FileName);
    }

    [NotNull]
    public static ISourceFile Empty { get; } = new EmptySourceFile();

    public string FileName { get; }

    public bool IsModified { get; set; }

    public DateTime LastWriteTimeUtc { get; }

    [NotNull]
    protected IFileSystemService FileSystem { get; }

    public string GetFileNameWithoutExtensions()
    {
      return PathHelper.GetDirectoryAndFileNameWithoutExtensions(this.FileName);
    }

    public string GetProjectPath(IProject project)
    {
      return PathHelper.UnmapPath(project.ProjectDirectory, this.FileName);
    }

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
      return this.FileSystem.ReadAllLines(this.FileName);
    }

    public string ReadAsText()
    {
      var contents = this.FileSystem.ReadAllText(this.FileName);
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
