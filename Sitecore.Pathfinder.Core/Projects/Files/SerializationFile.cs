namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class SerializationFile : File
  {
    public SerializationFile([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, snapshot)
    {
    }
  }
}
