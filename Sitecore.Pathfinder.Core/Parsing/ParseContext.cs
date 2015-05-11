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

  public class ParseContext : IParseContext
  {
    public ParseContext([NotNull] ICompositionService compositionService, [NotNull] IConfiguration configuration, [NotNull] ITokenService tokenService)
    {
      this.CompositionService = compositionService;
      this.Configuration = configuration;
      this.TokenService = tokenService;
    }

    public ICompositionService CompositionService { get; }

    public IConfiguration Configuration { get; }

    public virtual string DatabaseName => this.Project.DatabaseName;

    public virtual string ItemName
    {
      get
      {
        var s = this.SourceFile.SourceFileName.LastIndexOf('\\') + 1;
        var e = this.SourceFile.SourceFileName.IndexOf('.', s);

        return this.SourceFile.SourceFileName.Mid(s, e - s);
      }
    }

    public string ItemPath
    {
      get
      {
        var itemPath = this.GetRelativeFileName(this.SourceFile);

        itemPath = PathHelper.GetDirectoryAndFileNameWithoutExtensions(itemPath);

        return "/sitecore/" + PathHelper.NormalizeItemPath(itemPath);
      }
    }

    public IProject Project { get; private set; }

    public ISourceFile SourceFile { get; private set; }

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
      this.SourceFile = sourceFile;
      return this;
    }

    public string ReplaceTokens(string text)
    {
      var filePath = "/" + PathHelper.NormalizeItemPath(this.GetRelativeFileName(this.SourceFile));
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
