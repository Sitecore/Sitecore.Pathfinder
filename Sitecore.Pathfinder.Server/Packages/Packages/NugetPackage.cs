namespace Sitecore.Pathfinder.Packages.Packages
{
  using NuGet;

  public class NugetPackage : PackageBase
  {
    public NugetPackage([NotNull] IPackage package)
    {
      this.Package = package;
    }

    public override string Name => this.Package.Title;

    [NotNull]
    public IPackage Package { get; }

    public override string PackageId => this.Package.Id;

    public override string Status => "Available";

    public override SemanticVersion Version => this.Package.Version;
  }
}
