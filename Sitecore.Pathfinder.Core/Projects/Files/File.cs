namespace Sitecore.Pathfinder.Projects.Files
{
  using System.IO;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.IO;

  public class File : ProjectItem
  {
    private string filePath;

    private string shortName;

    public File([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, GetProjectUniqueId(project, snapshot), snapshot)
    {
    }

    public string FilePath => this.filePath ?? (this.filePath = PathHelper.GetFilePath(this.Project, this.Snapshot.SourceFile));

    public override string QualifiedName => this.Snapshot.SourceFile.FileName;

    public override string ShortName => this.shortName ?? (this.shortName = Path.GetFileName(this.Snapshot.SourceFile.FileName));

    public override void Rename(string newQualifiedName)
    {
      // this.Project.FileSystem.Rename();
    }

    [NotNull]
    private static string GetProjectUniqueId([NotNull] IProject project, [NotNull] ISnapshot snapshot)
    {
      return PathHelper.NormalizeItemPath(PathHelper.UnmapPath(project.Options.ProjectDirectory, snapshot.SourceFile.FileName));
    }
  }
}
