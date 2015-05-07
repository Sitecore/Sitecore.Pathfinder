namespace Sitecore.Pathfinder.Packages.Packages
{
  using NuGet;

  public abstract class PackageBase
  {
    public bool HasUpdate { get; set; }

    public bool IsInstalled { get; set; }

    [NotNull]
    public abstract string Name { get; }

    [NotNull]
    public abstract string PackageId { get; }

    [NotNull]
    public abstract string Status { get; }

    public SemanticVersion UpdateVersion { get; set; }

    [NotNull]
    public abstract SemanticVersion Version { get; }
  }
}
