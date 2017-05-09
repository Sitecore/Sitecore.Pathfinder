using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestClass]
    public class AttributeNameTextNodeTests : Tests
    {
        [TestInitialize]
        public void Setup()
        {
            Start(GoodWebsite);
        }

        [TestMethod]
        public void ItemTests()
        {

            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt");

            var doc = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, "<Item><Field Name=\"Text\" Value=\"123\" /></Item>", string.Empty, string.Empty);
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

            var mutableTextNode = (IMutableTextNode)attributeNameTextNode;

            mutableTextNode.SetKey("NewName");            
            Assert.AreEqual("NewName", attributeNameTextNode.Key);
            Assert.AreEqual("NewName", attributeNameTextNode.Value);

            mutableTextNode.SetValue("NewNameValue");
            Assert.AreEqual("NewNameValue", attributeNameTextNode.Key);
            Assert.AreEqual("NewNameValue", attributeNameTextNode.Value);
        }
    }
}
