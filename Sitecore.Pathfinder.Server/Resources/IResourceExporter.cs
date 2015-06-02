namespace Sitecore.Pathfinder.Resources
{
  using Sitecore.Zip;

  public interface IResourceExporter
  {
    void Export([NotNull] ZipWriter zip);
  }
}