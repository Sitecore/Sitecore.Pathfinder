namespace Sitecore.Pathfinder.Projects
{
  using System.IO;
  using System.Linq;
  using NUnit.Framework;
  using Sitecore.Pathfinder.Projects.Items;

  [TestFixture]
  public class ProjectTests : Tests
  {
    [TestFixtureSetUp]
    public void Startup()
    {
      this.StartupTests();
    }

    [Test]
    public void AddRemoveTests()
    {
      var project = new Project(this.Services.CompositionService, this.Services.Trace, this.Services.FileSystem, this.Services.ParseService).Load(this.ProjectDirectory, "master");

      var fileName = Path.Combine(this.ProjectDirectory, "content\\Home\\HelloWorld.item.xml");

      project.Add(fileName);
      Assert.AreEqual(1, project.SourceFiles.Count);
      Assert.AreEqual(fileName, project.SourceFiles.First().SourceFileName);

      project.Remove(fileName);
      Assert.AreEqual(0, project.SourceFiles.Count);
    }

    [Test]
    public void LoadProjectTests()
    {
      var project = this.Services.ProjectService.LoadProject();
      Assert.AreEqual(14, project.Items.Count);
      Assert.AreEqual(9, project.SourceFiles.Count);

      var projectItem = project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/HelloWorld");
      Assert.IsNotNull(projectItem);
      Assert.AreEqual("HelloWorld", projectItem.ShortName);
      Assert.AreEqual("/sitecore/content/Home/HelloWorld", projectItem.QualifiedName);

      var item = projectItem as Item;
      Assert.IsNotNull(item);
      Assert.AreEqual("HelloWorld", item.ItemName);
      Assert.AreEqual("/sitecore/content/Home/HelloWorld", item.ItemIdOrPath);
      Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath);

      var treeNode = projectItem.TreeNode;
      Assert.AreEqual("Item", treeNode.Name);
      Assert.AreEqual(1, treeNode.Attributes.Count);

      var attr = treeNode.Attributes.First();
      Assert.AreEqual("Template.Create", attr.Name);
      Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);
    }
  }
}