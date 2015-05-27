namespace Sitecore.Pathfinder.Projects.Files
{
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Documents;

  public class BinFile : File
  {
    public BinFile([NotNull] IProject project, [NotNull] ISnapshot snapshot) : base(project, snapshot)
    {
    }
  }
}
