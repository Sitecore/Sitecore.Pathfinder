// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Xml;

namespace Sitecore.Pathfinder.Snapshots.Xml
{
    [TestFixture]
    public class XmlDocumentTests : Tests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Start(GoodWebsite);
        }

        [Test]
        public void GetNestedTextNodeTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, "<Item><Field Name=\"Text\">123</Field></Item>", string.Empty, string.Empty);
            var root = doc.Root;

            var fields = root.GetSnapshotLanguageSpecificChildNode("Fields");
            Assert.IsNotNull(fields);

            var field = fields.ChildNodes.First();
            Assert.IsNotNull(field);
            Assert.AreEqual("Text", field.GetAttributeValue("Name"));
        }

        [Test]
        public void InvalidXmlTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, "<Item>", string.Empty, string.Empty);
            Assert.AreEqual(TextNode.Empty, doc.Root);

            doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, string.Empty, string.Empty, string.Empty);
            Assert.AreEqual(TextNode.Empty, doc.Root);
        }

        [Test]
        public void ItemTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, "<Item><Field Name=\"Text\" Value=\"123\" /></Item>", string.Empty, string.Empty);
            var root = doc.Root;
            Assert.IsNotNull(root);
            Assert.AreEqual("Item", root.Key);
            Assert.AreEqual(1, root.ChildNodes.Count());

            var field = root.ChildNodes.First();
            Assert.AreEqual("Field", field.Key);
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

        [Test]
        public void ValueTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, "<Item><Field Name=\"Text\">123</Field></Item>", string.Empty, string.Empty);
            var root = doc.Root;
            var field = root.ChildNodes.First();
            Assert.AreEqual("Field", field.Key);
            Assert.AreEqual("Text", field.GetAttributeValue("Name"));
            Assert.AreEqual("123", field.GetInnerTextNode()?.Value);
            Assert.AreEqual(string.Empty, field.Value);
            Assert.AreEqual(0, field.ChildNodes.Count());
        }
    }
}
