// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Snapshots
{
    [TestClass]
    public class SourceFileTests : Tests
    {
        [Diagnostics.NotNull]
        public IProjectBase Project { get; set; }

        [TestMethod]
        public void ConstructorTest()
        {
            Start();
            Project = Services.ProjectService.LoadProjectFromConfiguration();

            var projectFileName = "~/items/yaml/ribbon.content.yaml";
            var fileName = Path.Combine(ProjectDirectory, "items\\yaml\\ribbon.content.yaml");
            var sourceFile = new SourceFile(Services.FileSystem, fileName, fileName, projectFileName);

            Assert.AreEqual(fileName, sourceFile.AbsoluteFileName);
            Assert.AreNotEqual(DateTime.MinValue, sourceFile.LastWriteTimeUtc);
            Assert.AreEqual("~/items/yaml/ribbon.content.yaml", sourceFile.ProjectFileName);
        }
    }
}
