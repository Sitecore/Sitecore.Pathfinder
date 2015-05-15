namespace Sitecore.Pathfinder.Projects
{
  using System.IO;
  using System.Linq;
  using NUnit.Framework;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.Projects.Items;
  using Sitecore.Pathfinder.Projects.Templates;

  [TestFixture]
  public class ProjectTests : Tests
  {
    [NotNull]
    public IProject Project { get; set; }

    [TestFixtureSetUp]
    public void Startup()
    {
      this.StartupTests();
      this.Project = this.Services.ProjectService.LoadProject();
    }

    [Test]
    public void AddRemoveTests()
    {
      var project = new Project(this.Services.CompositionService, this.Services.Configuration, this.Services.Trace, this.Services.FileSystem, this.Services.ParseService).Load(this.ProjectDirectory, "master");

      var fileName = Path.Combine(this.ProjectDirectory, "content\\Home\\HelloWorld.item.xml");

      project.Add(fileName);
      Assert.AreEqual(1, project.SourceFiles.Count);
      Assert.AreEqual(fileName, project.SourceFiles.First().SourceFileName);

      project.Remove(fileName);
      Assert.AreEqual(0, project.SourceFiles.Count);
    }

    [Test]
    public void JsonItemTest()
    {
      var projectItem = this.Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/Foo");
      Assert.IsNotNull(projectItem);
      Assert.AreEqual("Foo", projectItem.ShortName);
      Assert.AreEqual("/sitecore/content/Home/Foo", projectItem.QualifiedName);

      var item = projectItem as Item;
      Assert.IsNotNull(item);
      Assert.AreEqual("Foo", item.ItemName);
      Assert.AreEqual("/sitecore/content/Home/Foo", item.ItemIdOrPath);
      Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath);

      var treeNode = projectItem.TextNode;
      Assert.AreEqual("Item", treeNode.Name);
      Assert.AreEqual(1, treeNode.Attributes.Count);

      var attr = treeNode.Attributes.First();
      Assert.AreEqual("Template.Create", attr.Name);
      Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);

      var field = item.Fields.FirstOrDefault(f => f.Name == "Title");
      Assert.IsNotNull(field);
      Assert.AreEqual("Hello", field.Value);
    }

    [Test]
    public void JsonTemplateTest()
    {
      var projectItem = this.Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/FooTemplate");
      Assert.IsNotNull(projectItem);
      Assert.AreEqual("FooTemplate", projectItem.ShortName);
      Assert.AreEqual("/sitecore/content/Home/FooTemplate", projectItem.QualifiedName);

      var template = projectItem as Template;
      Assert.IsNotNull(template);
      Assert.AreEqual("FooTemplate", template.ItemName);
      Assert.AreEqual("Applications/16x16/About.png", template.Icon);
      Assert.AreEqual("ShortHelp", template.ShortHelp);
      Assert.AreEqual("LongHelp", template.LongHelp);
      Assert.AreEqual("/sitecore/content/Home/FooTemplate", template.ItemIdOrPath);
      Assert.AreEqual(1, template.Sections.Count);

      var section = template.Sections[0];
      Assert.AreEqual("Fields", section.Name);
      Assert.AreEqual("Applications/16x16/About.png", section.Icon);
      Assert.AreEqual(2, section.Fields.Count);

      var field = section.Fields[0];
      Assert.AreEqual("Title", field.Name);
      Assert.AreEqual("Single-Line Text", field.Type);
      Assert.AreEqual("ShortHelp", field.ShortHelp);
      Assert.AreEqual("LongHelp", field.LongHelp);
      Assert.AreEqual("StandardValue", field.StandardValue);

      Assert.AreEqual("Rich Text", section.Fields[1].Type);
    }

    [Test]
    public void LoadProjectTests()
    {
      Assert.IsTrue(this.Project.Items.Any());
      Assert.IsTrue(this.Project.SourceFiles.Any());
    }

    [Test]
    public void XmlItemTest()
    {
      var projectItem = this.Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/HelloWorld");
      Assert.IsNotNull(projectItem);
      Assert.AreEqual("HelloWorld", projectItem.ShortName);
      Assert.AreEqual("/sitecore/content/Home/HelloWorld", projectItem.QualifiedName);

      var item = projectItem as Item;
      Assert.IsNotNull(item);
      Assert.AreEqual("HelloWorld", item.ItemName);
      Assert.AreEqual("/sitecore/content/Home/HelloWorld", item.ItemIdOrPath);
      Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath);

      var field = item.Fields.FirstOrDefault(f => f.Name == "Title");
      Assert.IsNotNull(field);
      Assert.AreEqual("Hello", field.Value);

      var treeNode = projectItem.TextNode;
      Assert.AreEqual("Item", treeNode.Name);
      Assert.AreEqual(1, treeNode.Attributes.Count);

      var attr = treeNode.Attributes.First();
      Assert.AreEqual("Template.Create", attr.Name);
      Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);
    }
  }
}
