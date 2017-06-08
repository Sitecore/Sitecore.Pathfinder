// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Json;

namespace Sitecore.Pathfinder.Snapshots.Json
{
    [TestClass]
    public class JsonDocumentTests : Tests
    {
        [TestInitialize]
        public void Setup()
        {
            Start();
        }

        [TestMethod]
        public void GetNestedTextNodeTests()
        {
            var sourceFile = new SourceFile(Services.Configuration, Services.FileSystem, "test.txt");

            var doc = Services.Factory.JsonTextSnapshot(sourceFile, "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }");
            var root = doc.Root;

            var fields = root;
            Assert.IsNotNull(fields);

            var field = fields.ChildNodes.FirstOrDefault();
            Assert.IsNotNull(field);
            Assert.AreEqual("Text", field.GetAttributeValue("Name"));
        }

        [TestMethod]
        public void InvalidJsonTests()
        {
            var sourceFile = new SourceFile(Services.Configuration, Services.FileSystem, "test.txt");

            var doc = Services.Factory.JsonTextSnapshot(sourceFile, "\"Item\": { }");
            Assert.AreEqual(TextNode.Empty, doc.Root);

            doc = Services.Factory.JsonTextSnapshot(sourceFile, string.Empty);
            Assert.AreEqual(TextNode.Empty, doc.Root);
        }

        [TestMethod]
        public void ItemTests()
        {
            var sourceFile = new SourceFile(Services.Configuration, Services.FileSystem, "test.txt");

            var doc = Services.Factory.JsonTextSnapshot(sourceFile, "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }");
            var root = doc.Root;
            Assert.IsNotNull(root);
            Assert.AreEqual("Item", root.Key);
            Assert.AreEqual(1, root.ChildNodes.Count());

            var fields = root.ChildNodes;

            var field = fields.First();
            Assert.AreEqual("Text", field.GetAttributeValue("Name"));
            Assert.AreEqual("123", field.GetAttributeValue("Value"));
            Assert.AreEqual(0, field.ChildNodes.Count());

            var attribute = field.GetAttribute("Name");
            Assert.IsNotNull(attribute);
            Assert.AreEqual("Text", attribute.Value);
            Assert.AreEqual(0, attribute.Attributes.Count());
            Assert.AreEqual(0, attribute.ChildNodes.Count());
            Assert.AreEqual(field.Snapshot, attribute.Snapshot);
            Assert.AreEqual(doc, attribute.Snapshot);
        }
    }
}
