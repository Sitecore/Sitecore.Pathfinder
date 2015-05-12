namespace Sitecore.Pathfinder.Parsing
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.Composition;
  using System.IO;
  using Microsoft.Framework.ConfigurationModel;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.StringExtensions;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Projects;
  using Sitecore.Pathfinder.TreeNodes;

  public class ParseContext : IParseContext
  {
    public ParseContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] IProjectService projectService, [NotNull] ITokenService tokenService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.ProjectService = projectService;
      this.TokenService = tokenService;
    }

    public ICompositionService CompositionService { get; }

    public IConfiguration Configuration { get; }

    [NotNull]
    protected IProjectService ProjectService { get;  }

    public virtual string DatabaseName => this.Project.DatabaseName;

    public virtual string ItemName
    {
      get
      {
        var fileName = this.Document.SourceFile.SourceFileName;

        var s = fileName.LastIndexOf('\\') + 1;
        var e = fileName.IndexOf('.', s);

        return fileName.Mid(s, e - s);
      }
    }

    public string ItemPath
    {
      get
      {
        var itemPath = this.GetRelativeFileName(this.Document.SourceFile);

        itemPath = PathHelper.GetDirectoryAndFileNameWithoutExtensions(itemPath);

        return "/sitecore/" + PathHelper.NormalizeItemPath(itemPath);
      }
    }

    public IProject Project { get; private set; }

    public IDocument Document { get; private set; }

    [NotNull]
    protected ITokenService TokenService { get; }

    public virtual string GetRelativeFileName(ISourceFile sourceFile)
    {
      if (sourceFile.SourceFileName.StartsWith(this.Project.ProjectDirectory, StringComparison.OrdinalIgnoreCase))
      {
        return sourceFile.SourceFileName.Mid(this.Project.ProjectDirectory.Length + 1);
      }

      return sourceFile.SourceFileName;
    }

    public IParseContext Load(IProject project, ISourceFile sourceFile)
    {
      this.Project = project;
      this.Document = this.ProjectService.LoadDocument(sourceFile);
      return this;
    }

    public string ReplaceTokens(string text)
    {
      var filePath = "/" + PathHelper.NormalizeItemPath(this.GetRelativeFileName(this.Document.SourceFile));
      var filePathWithExtensions = PathHelper.NormalizeItemPath(PathHelper.GetDirectoryAndFileNameWithoutExtensions(filePath));
      var fileName = Path.GetFileName(filePath);
      var fileNameWithoutExtensions = PathHelper.GetFileNameWithoutExtensions(fileName);
      var directoryName = PathHelper.NormalizeFilePath(Path.GetDirectoryName(filePath) ?? string.Empty);

      var contextTokens = new Dictionary<string, string>
      {
        ["ItemPath"] = this.ItemPath, 
        ["FilePathWithoutExtensions"] = filePathWithExtensions, 
        ["FilePath"] = filePath, 
        ["Database"] = this.DatabaseName, 
        ["FileNameWithoutExtensions"] = fileNameWithoutExtensions, 
        ["FileName"] = fileName, 
        ["DirectoryName"] = directoryName, 
      };

      return this.TokenService.Replace(text, contextTokens);
    }
  }
}
