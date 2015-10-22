using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestFixture]
    public class AttributeNameTextNodeTests : Tests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Start(GoodWebsite);
        }

        [Test]
        public void ItemTests()
        {

            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(sourceFile, "<Item><Field Name=\"Text\" Value=\"123\" /></Item>", SnapshotParseContext.Empty, string.Empty, string.Empty);
            Assert.AreEqual("Item", doc.Root.Key);

            var field = doc.Root.ChildNodes.First();
            
            var attributeNameTextNode = new AttributeNameTextNode(field.GetAttribute("Name"));
            Assert.AreEqual(0, attributeNameTextNode.Attributes.Count());
            Assert.AreEqual(0, attributeNameTextNode.ChildNodes.Count());
            Assert.AreEqual("Name", attributeNameTextNode.Key);
            Assert.AreEqual(1, attributeNameTextNode.TextSpan.LineNumber);
            Assert.AreEqual(field.Snapshot, attributeNameTextNode.Snapshot);
            Assert.AreEqual(14, attributeNameTextNode.TextSpan.LinePosition);

            Assert.AreEqual(null, attributeNameTextNode.GetAttribute("None"));
            Assert.AreEqual(string.Empty, attributeNameTextNode.GetAttributeValue("None"));
            Assert.AreEqual(null, attributeNameTextNode.GetInnerTextNode());

            attributeNameTextNode.SetKey("NewName");            
            Assert.AreEqual("NewName", attributeNameTextNode.Key);
            Assert.AreEqual("NewName", attributeNameTextNode.Value);

            attributeNameTextNode.SetValue("NewNameValue");
            Assert.AreEqual("NewNameValue", attributeNameTextNode.Key);
            Assert.AreEqual("NewNameValue", attributeNameTextNode.Value);
        }
    }
}
