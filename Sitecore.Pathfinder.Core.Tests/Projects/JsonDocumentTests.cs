namespace Sitecore.Pathfinder.Projects
{
  using NUnit.Framework;
  using Sitecore.Pathfinder.Diagnostics;
  using Sitecore.Pathfinder.IO;
  using Sitecore.Pathfinder.Parsing;
  using Sitecore.Pathfinder.TextDocuments;
  using Sitecore.Pathfinder.TextDocuments.Json;
  using Sitecore.Pathfinder.TextDocuments.Xml;

  [TestFixture]
  public class JsonDocumentTests : Tests
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
      var parseContext = this.Resolve<IParseContext>().With(project, sourceFile);

      this.MockedFileSystem.Contents = "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }";

      var doc = new JsonTextDocument(parseContext, sourceFile);
      var root = doc.Root;
      Assert.IsNotNull(root);
      Assert.AreEqual("Item", root.Name);
      Assert.AreEqual(1, root.ChildNodes.Count);

      var fields = root.ChildNodes[0];
      Assert.AreEqual(1, fields.ChildNodes.Count);

      var field = fields.ChildNodes[0];
      Assert.AreEqual("Text", field.GetAttributeValue("Name"));
      Assert.AreEqual("123", field.GetAttributeValue("Value"));
      Assert.AreEqual(0, field.ChildNodes.Count);
    }

    [Test]
    public void InvalidJsonTests()
    {
      var project = this.Resolve<IProject>();
      var sourceFile = new SourceFile(project.FileSystem, "test.txt");
      var parseContext = this.Resolve<IParseContext>().With(project, sourceFile);

      this.MockedFileSystem.Contents = "\"Item\": { }";
      var doc = new JsonTextDocument(parseContext, sourceFile);
      Assert.Throws<BuildException>(() => { var r = doc.Root.ChildNodes[0]; });

      this.MockedFileSystem.Contents = string.Empty;
      doc = new JsonTextDocument(parseContext, sourceFile);
      Assert.Throws<BuildException>(() => { var r = doc.Root.ChildNodes[0]; });
    }
  }
}