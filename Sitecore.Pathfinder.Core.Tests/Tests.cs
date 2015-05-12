namespace Sitecore.Pathfinder
{
  using System.IO;
  using System.Reflection;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Helpers;

  public abstract class Tests
  {
    [NotNull]
    public string ProjectDirectory { get; private set; }

    [NotNull]
    public Services Services { get; private set; }

    protected void StartupTests()
    {
      this.Services = new Services();
      this.Services.Start();
      this.Services.ConfigurationService.Load(LoadConfigurationOptions.None);

      this.ProjectDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Website");
    }
  }
}