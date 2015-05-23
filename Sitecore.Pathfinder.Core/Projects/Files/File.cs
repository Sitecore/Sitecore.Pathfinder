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

    public File([NotNull] IProject project, [NotNull] IDocumentSnapshot documentSnapshot) : base(project, GetProjectUniqueId(project, documentSnapshot), documentSnapshot)
    {
    }

    public override string QualifiedName => this.qualifiedName ?? (this.qualifiedName = this.DocumentSnapshot.SourceFile.FileName);

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.DocumentSnapshot.SourceFile.FileName));

    [NotNull]
    private static string GetProjectUniqueId([NotNull] IProject project, [NotNull] IDocumentSnapshot documentSnapshot)
    {
      return PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, documentSnapshot.SourceFile.FileName));
    }
  }
}
