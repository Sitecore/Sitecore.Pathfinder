namespace Sitecore.Pathfinder.Projects
{
  using System.Collections.Generic;
  using Sitecore.Pathfinder.Diagnostics;

  public class ProjectOptions
  {
    public static readonly ProjectOptions Empty = new ProjectOptions(string.Empty, string.Empty);

    public ProjectOptions([NotNull] string projectDirectory, [NotNull] string databaseName)
    {
      this.ProjectDirectory = projectDirectory;
      this.DatabaseName = databaseName;
    }

    [NotNull]
    public string DatabaseName { get; }

    [NotNull]
    public ICollection<string> ExternalReferences { get; } = new List<string>();

    [NotNull]
    public string ProjectDirectory { get; }

    [NotNull]
    public IDictionary<string, string> RemapFileDirectories { get; } = new Dictionary<string, string>();
  }
}
