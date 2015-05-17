namespace Sitecore.Pathfinder.TextDocuments.Json
{
  using NUnit.Framework;
  using Sitecore.Pathfinder.Diagnostics;

  [TestFixture]
  public class JsonDocumentTests : Tests
  {
    [TestFixtureSetUp]
    public void Setup()
    {
      this.Start();
    }

    [Test]
    public void InvalidJsonTests()
    {
      var sourceFile = new SourceFile(this.Services.FileSystem, "test.txt");

      var doc = new JsonTextDocument(sourceFile, "\"Item\": { }");
      Assert.Throws<BuildException>(() => { var r = doc.Root.ChildNodes[0]; });

      doc = new JsonTextDocument(sourceFile, string.Empty);
      Assert.Throws<BuildException>(() => { var r = doc.Root.ChildNodes[0]; });
    }

    [Test]
    public void ItemTests()
    {
      var sourceFile = new SourceFile(this.Services.FileSystem, "test.txt");

      var doc = new JsonTextDocument(sourceFile, "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }");
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
  }
}
