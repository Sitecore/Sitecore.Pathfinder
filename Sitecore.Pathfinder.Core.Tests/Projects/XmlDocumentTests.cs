namespace Sitecore.Pathfinder.Projects
{
  using NUnit.Framework;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.TextDocuments;
  using Sitecore.Pathfinder.TextDocuments.Xml;

  [TestFixture]
  public class XmlDocumentTests : Tests
  {
    [NotNull]
    private MockedFileSystemService MockedFileSystem { get; } = new MockedFileSystemService();

    [TestFixtureSetUp]
    public void Setup()
    {
      this.Start(() => { this.Mock<IFileSystemService>(this.MockedFileSystem); });
    }

    [Test]
    public void ItemTests()
    {
      var project = this.Resolve<IProject>();
      var sourceFile = new SourceFile(project.FileSystem, "test.txt");
      var context = this.Resolve<IParseContext>().With(project, sourceFile);

      this.MockedFileSystem.Contents = "<Item><Field Name=\"Text\" Value=\"123\" /></Item>";

      var doc = new XmlTextDocument(context, sourceFile);
      var root = doc.Root;
      Assert.IsNotNull(root);
      Assert.AreEqual("Item", root.Name);
      Assert.AreEqual(1, root.ChildNodes.Count);

      var field = root.ChildNodes[0];
      Assert.AreEqual("Field", field.Name);
      Assert.AreEqual("Text", field.GetAttributeValue("Name"));
      Assert.AreEqual("123", field.GetAttributeValue("Value"));
      Assert.AreEqual(0, field.ChildNodes.Count);
    }

    [Test]
    public void ValueTests()
    {
      var project = this.Resolve<IProject>();
      var sourceFile = new SourceFile(project.FileSystem, "test.txt");
      var context = this.Resolve<IParseContext>().With(project, sourceFile);

      this.MockedFileSystem.Contents = "<Item><Field Name=\"Text\">123</Field></Item>";

      var doc = new XmlTextDocument(context, sourceFile);
      var root = doc.Root;
      var field = root.ChildNodes[0];
      Assert.AreEqual("Field", field.Name);
      Assert.AreEqual("Text", field.GetAttributeValue("Name"));
      Assert.AreEqual("123", field.Value);
      Assert.AreEqual(0, field.ChildNodes.Count);
    }

    [Test]
    public void InvalidXmlTests()
    {
      var project = this.Resolve<IProject>();
      var sourceFile = new SourceFile(project.FileSystem, "test.txt");
      var context = this.Resolve<IParseContext>().With(project, sourceFile);

      this.MockedFileSystem.Contents = "<Item>";
      var doc = new XmlTextDocument(context, sourceFile);
      Assert.Throws<BuildException>(() => { var r = doc.Root.ChildNodes[0]; });

      this.MockedFileSystem.Contents = string.Empty;
      doc = new XmlTextDocument(context, sourceFile);
      Assert.Throws<BuildException>(() => { var r = doc.Root.ChildNodes[0]; });
    }
  }
}