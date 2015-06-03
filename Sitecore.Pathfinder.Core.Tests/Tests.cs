namespace Sitecore.Pathfinder
{
  using System;
  using System.IO;
  using System.Reflection;
  using Sitecore.Pathfinder.Configuration;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions;
  using Sitecore.Pathfinder.Helpers;

  public abstract class Tests
  {
    [NotNull]
    public string ProjectDirectory { get; private set; }

    [NotNull]
    public Services Services { get; private set; }

    protected void Mock<T>(T value)
    {
      this.Services.CompositionService.Set<T>(value);
    }

    protected T Resolve<T>()
    {
      return this.Services.CompositionService.Resolve<T>();
    }

    protected void Start([CanBeNull] Action mock = null)
    {
      this.Services = new Services();
      this.Services.Start(mock);
      this.Services.ConfigurationService.Load(LoadConfigurationOptions.None);

      this.ProjectDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Website");
    }
  }
}