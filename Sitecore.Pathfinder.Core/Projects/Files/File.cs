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

    public File([NotNull] IProject project, [NotNull] ITextNode textNode) : base(project, GetProjectUniqueId(project, textNode), textNode)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.TextNode.TextDocument.SourceFile.SourceFileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.TextNode.TextDocument.SourceFile.SourceFileName));

    public override void Bind()
    {
    }

    [NotNull]
    private static string GetProjectUniqueId([NotNull] IProject project, [NotNull] ITextNode textNode)
    {
      return PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.ProjectDirectory, textNode.TextDocument.SourceFile.SourceFileName));
    }
  }
}