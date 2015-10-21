// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Json;

namespace Sitecore.Pathfinder.Snapshots.Json
{
    [TestFixture]
    public class JsonDocumentTests : Tests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Start(GoodWebsite);
        }

        [Test]
        public void GetNestedTextNodeTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<JsonTextSnapshot>().With(sourceFile, "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }", new Dictionary<string, string>());
            var root = doc.Root;

            var fields = root.GetLogicalChildNode("Fields");
            Assert.IsNotNull(fields);

            var field = fields.ChildNodes.FirstOrDefault();
            Assert.IsNotNull(field);
            Assert.AreEqual("Text", field.GetAttributeValue("Name"));
        }

        [Test]
        public void InvalidJsonTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<JsonTextSnapshot>().With(sourceFile, "\"Item\": { }", new Dictionary<string, string>());
            Assert.AreEqual(TextNode.Empty, doc.Root);

            doc = Services.CompositionService.Resolve<JsonTextSnapshot>().With(sourceFile, string.Empty, new Dictionary<string, string>());
            Assert.AreEqual(TextNode.Empty, doc.Root);
        }

        [Test]
        public void ItemTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<JsonTextSnapshot>().With(sourceFile, "{ \"Item\": { \"Fields\": [ { \"Name\": \"Text\", \"Value\": \"123\" } ] } }", new Dictionary<string, string>());
            var root = doc.Root;
            Assert.IsNotNull(root);
            Assert.AreEqual("Item", root.Key);
            Assert.AreEqual(1, root.ChildNodes.Count());

            var fields = root.ChildNodes.First();
            Assert.AreEqual(1, fields.ChildNodes.Count());

            var field = fields.ChildNodes.First();
            Assert.AreEqual("Text", field.GetAttributeValue("Name"));
            Assert.AreEqual("123", field.GetAttributeValue("Value"));
            Assert.AreEqual(0, field.ChildNodes.Count());

            var attribute = field.GetAttribute("Name");
            Assert.IsNotNull(attribute);
            Assert.AreEqual("Text", attribute.Value);
            Assert.AreEqual(0, attribute.Attributes.Count());
            Assert.AreEqual(0, attribute.ChildNodes.Count());
            Assert.AreEqual(field, attribute.ParentNode);
            Assert.AreEqual(field.Snapshot, attribute.Snapshot);
            Assert.AreEqual(doc, attribute.Snapshot);
        }
    }
}
