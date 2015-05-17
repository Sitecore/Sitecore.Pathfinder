namespace Sitecore.Pathfinder.Projects.Files
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.TextDocuments;

  public class File : ProjectItem
  {
    private string qualifiedName;

    private string shortName;

    public File([NotNull] IProject project, [NotNull] IDocument document) : base(project, GetProjectUniqueId(project, document), document)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.Document.SourceFile.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.Document.SourceFile.SourceFileName));

    public override void Bind()
    {
    }

    [NotNull]
    private static string GetProjectUniqueId([NotNull] IProject project, [NotNull] IDocument document)
    {
      return PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.ProjectDirectory, document.SourceFile.SourceFileName));
    }
  }
}
