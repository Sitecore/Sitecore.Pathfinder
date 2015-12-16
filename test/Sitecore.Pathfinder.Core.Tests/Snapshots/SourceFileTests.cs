// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using NUnit.Framework;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestFixture]
    public class SourceFileTests : Tests
    {
        [NotNull]
        public IProject Project { get; set; }

        [Test]
        public void ConstructorTest()
        {
            Start(GoodWebsite);
            Project = Services.ProjectService.LoadProjectFromConfiguration();

            var projectFileName = "~/content/Home/XmlItem.item.xml";
            var fileName = Path.Combine(ProjectDirectory, "content\\Home\\XmlItem.item.xml");
            var sourceFile = new SourceFile(Services.FileSystem, fileName, projectFileName);

            Assert.AreEqual(fileName, sourceFile.AbsoluteFileName);
            Assert.AreNotEqual(DateTime.MinValue, sourceFile.LastWriteTimeUtc);
            Assert.AreEqual("~/content/Home/XmlItem.item.xml", sourceFile.ProjectFileName);
        }
    }
}
