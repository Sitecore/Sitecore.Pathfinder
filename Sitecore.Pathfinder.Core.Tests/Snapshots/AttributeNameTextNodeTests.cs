using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Snapshots.Xml;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestFixture]
    public class AttributeNameTextNodeTests : Tests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Start();
        }

        [Test]
        public void ItemTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt");

            var doc = new XmlTextSnapshot(sourceFile, "<Item><Field Name=\"Text\" Value=\"123\" /></Item>", string.Empty, string.Empty);
            Assert.AreEqual("Item", doc.Root.Name);

            var field = doc.Root.ChildNodes.First();
            
            var attributeNameTextNode = new AttributeNameTextNode(field.GetAttributeTextNode("Name"));
            Assert.AreEqual(0, attributeNameTextNode.Attributes.Count());
            Assert.AreEqual(0, attributeNameTextNode.ChildNodes.Count());
            Assert.AreEqual("Name", attributeNameTextNode.Name);
            Assert.AreEqual(field, attributeNameTextNode.Parent);
            Assert.AreEqual(1, attributeNameTextNode.Position.LineNumber);
            Assert.AreEqual(field.Snapshot, attributeNameTextNode.Snapshot);
            Assert.AreEqual(14, attributeNameTextNode.Position.LinePosition);

            Assert.AreEqual(null, attributeNameTextNode.GetAttributeTextNode("None"));
            Assert.AreEqual(string.Empty, attributeNameTextNode.GetAttributeValue("None"));
            Assert.AreEqual(null, attributeNameTextNode.GetInnerTextNode());

            attributeNameTextNode.SetName("NewName");            
            Assert.AreEqual("NewName", attributeNameTextNode.Name);
            Assert.AreEqual("NewName", attributeNameTextNode.Value);

            attributeNameTextNode.SetValue("NewNameValue");
            Assert.AreEqual("NewNameValue", attributeNameTextNode.Name);
            Assert.AreEqual("NewNameValue", attributeNameTextNode.Value);
        }
    }
}
