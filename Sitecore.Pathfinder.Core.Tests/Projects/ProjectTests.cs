namespace Sitecore.Pathfinder.Projects
{
  using System.ComponentModel.Composition;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using NUnit.Framework;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Extensions.CompositionServiceExtensions;
  using Sitecore.Pathfinder.Helpers;

  [TestFixture]
  public class ProjectTests
  {
    [NotNull]
    protected ICompositionService CompositionService { get; private set; }

    [TestFixtureSetUp]
    public void Startup()
    {
      this.CompositionService = CompositionServiceHelper.RegisterCompositionService();
    }

    [Test]
    public void AddRemoveTests()
    {
      var projectDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Website");

      var project = this.CompositionService.Resolve<IProject>();

      var fileName = Path.Combine(projectDirectory, "Website\\content\\Home\\HelloWorld.item.xml");

      project.Add(fileName);
      Assert.AreEqual(1, project.SourceFiles.Count);
      Assert.AreEqual(fileName, project.SourceFiles.First().SourceFileName);

      project.Remove(fileName);
      Assert.AreEqual(0, project.SourceFiles.Count);
    }
  }
}
