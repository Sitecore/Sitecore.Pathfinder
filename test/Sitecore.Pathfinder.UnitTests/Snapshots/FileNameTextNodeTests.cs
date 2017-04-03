// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestClass]
    public class FileNameTextNodeTests : Tests
    {
        [TestInitialize]
        public void Setup()
        {
            Start(GoodWebsite);
        }

        [TestMethod]
        public void FileNameTests()
        {
            var sourceFile = new SourceFile(Services.FileSystem, "test.txt", "test.txt", "test.txt");

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
