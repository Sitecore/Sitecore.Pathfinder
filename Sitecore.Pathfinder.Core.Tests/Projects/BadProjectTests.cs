// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public class BadProjectTests : Tests
    {
        [NotNull]
        public IProject Project { get; set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start("Website.Bad");
            Project = Services.ProjectService.LoadProjectFromConfiguration();
        }

        [Test]
        public void BadLinkTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/BadLink");
            Assert.IsNotNull(projectItem);

            var diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Reference not found: /sitecore/media library/badlink");
            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(Severity.Warning, diagnostic.Severity);
            Assert.AreEqual(2, diagnostic.Position.LineNumber);
            Assert.AreEqual(6, diagnostic.Position.LinePosition);
            Assert.AreEqual(5, diagnostic.Position.LineLength);
        }
    }
}
