using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Xml;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestFixture]
    public class FileNameTextNodeTests : Tests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Start(GoodWebsite);
        }

        [Test]
        public void FileNameTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt");

            var snapshot = Services.CompositionService.Resolve<XmlTextSnapshot>().With(SnapshotParseContext.Empty, sourceFile, "<Item><Field Name=\"Text\" Value=\"123\" /></Item>", string.Empty, string.Empty);
            Assert.AreEqual("Item", snapshot.Root.Key);

            var field = snapshot.Root.ChildNodes.First();
            
            var fileNameTextNode = new FileNameTextNode("test", snapshot);
            Assert.AreEqual(0, fileNameTextNode.Attributes.Count());
            Assert.AreEqual(0, fileNameTextNode.ChildNodes.Count());
            Assert.AreEqual("test", fileNameTextNode.Key);
            Assert.AreEqual(0, fileNameTextNode.TextSpan.LineNumber);
            Assert.AreEqual(0, fileNameTextNode.TextSpan.LinePosition);
            Assert.AreEqual(field.Snapshot, fileNameTextNode.Snapshot);

            Assert.AreEqual(null, fileNameTextNode.GetAttribute("None"));
            Assert.AreEqual(string.Empty, fileNameTextNode.GetAttributeValue("None"));
            Assert.AreEqual(null, fileNameTextNode.GetInnerTextNode());
        }
    }
}