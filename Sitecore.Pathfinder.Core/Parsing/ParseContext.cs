namespace Sitecore.Pathfinder.Parsing
{
  using System;
  using System.ComponentModel.Composition;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;

  public class ParseContext : IParseContext
  {
    public ParseContext([NotNull] ICompositionService compositionService, [NotNull] IProject project, [NotNull] ISourceFile sourceFile)
    {
      this.CompositionService = compositionService;
      this.Project = project;
      this.SourceFile = sourceFile;
    }

    public ICompositionService CompositionService { get; }

    public virtual string DatabaseName => this.Project.DatabaseName;

    public virtual string ItemName
    {
      get
      {
        var s = this.SourceFile.SourceFileName.LastIndexOf('\\') + 1;
        var e = this.SourceFile.SourceFileName.IndexOf('.', s);

        return this.SourceFile.SourceFileName.Mid(s, e - s - 1);
      }
    }

    public string ItemPath
    {
      get
      {
        var itemPath = this.GetRelativeFileName(this.SourceFile);

        itemPath = PathHelper.GetDirectoryAndFileNameWithoutExtensions(itemPath);

        return PathHelper.NormalizeItemPath(itemPath);
      }
    }

    public IProject Project { get; }

    public ISourceFile SourceFile { get; }

    public virtual string GetRelativeFileName(ISourceFile sourceFile)
    {
      if (!sourceFile.SourceFileName.StartsWith(this.Project.ProjectDirectory, StringComparison.OrdinalIgnoreCase))
      {
        return sourceFile.SourceFileName;
      }

      return sourceFile.SourceFileName.Mid(this.Project.ProjectDirectory.Length + 1);
    }
  }
}
