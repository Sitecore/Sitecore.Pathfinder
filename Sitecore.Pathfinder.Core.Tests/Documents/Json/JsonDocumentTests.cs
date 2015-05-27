namespace Sitecore.Pathfinder.TextDocuments.Json
{
  using NUnit.Framework;
  using Sitecore.Pathfinder.Documents;
  using Sitecore.Pathfinder.Documents.Json;

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

      var doc = new JsonTextSnapshot(sourceFile, "\"Item\": { }");
      Assert.AreEqual(TextNode.Empty, doc.Root);

      doc = new JsonTextSnapshot(sourceFile, string.Empty);
      Assert.AreEqual(TextNode.Empty, doc.Root);
    }

    [Test]
    public void ItemTests()
    {
      var sourceFile = new SourceFile(this.Services.FileSystem, "test.txt");

      var doc = new JsonTextSnapshot(sourceFile, "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }");
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
