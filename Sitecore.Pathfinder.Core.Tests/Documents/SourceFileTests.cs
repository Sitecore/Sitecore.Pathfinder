// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using NUnit.Framework;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Documents
{
    [TestFixture]
    public class SourceFileTests : Tests
    {
        [Test]
        public void ConstructorTest()
        {
            Start();

            var fileName = Path.Combine(ProjectDirectory, "content\\Home\\HelloWorld.item.xml");
            var sourceFile = new SourceFile(Services.FileSystem, fileName);

            Assert.AreEqual(fileName, sourceFile.FileName);
            Assert.AreNotEqual(DateTime.MinValue, sourceFile.LastWriteTimeUtc);
        }
    }
}
