// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;

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
        public void XmlItemTest()
        {
            var item = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/XmlItem") as Item;
            Assert.IsNotNull(item);

            var diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Reference not found: /sitecore/media library/badlink");
            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(Severity.Warning, diagnostic.Severity);
            Assert.AreEqual(2, diagnostic.Position.LineNumber);
            Assert.AreEqual(6, diagnostic.Position.LinePosition);
            Assert.AreEqual(5, diagnostic.Position.LineLength);

            // link field
            var linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("/sitecore/media library/badlink", linkField.Value);
            linkField.Resolve();
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
            Assert.AreEqual(1, linkField.Diagnostics.Count);
          
            // link field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink1");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("/sitecore/media library/badlink", linkField.Value);
            linkField.Resolve();
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
            Assert.AreEqual(1, linkField.Diagnostics.Count);
          
            // link field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink2");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("", linkField.Value);
            linkField.Resolve();
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
            Assert.AreEqual(0, linkField.Diagnostics.Count);
          
            // multiple links field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "Multiple");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("/sitecore/media library/badlink|/sitecore/media library/badlink", linkField.Value);
            linkField.Resolve();
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
            Assert.AreEqual(2, linkField.Diagnostics.Count);
          
            // image field
            var imageField = item.Fields.FirstOrDefault(f => f.FieldName == "BadImage");
            Assert.IsNotNull(imageField);
            Assert.AreEqual("/sitecore/media library/badlink", imageField.Value);
            imageField.Resolve();
            Assert.AreEqual(string.Empty, imageField.ResolvedValue);
            Assert.AreEqual(1, imageField.Diagnostics.Count);
          
            // image field
            imageField = item.Fields.FirstOrDefault(f => f.FieldName == "BadImage2");
            Assert.IsNotNull(imageField);
            Assert.AreEqual(string.Empty, imageField.Value);
            imageField.Resolve();
            Assert.AreEqual(string.Empty, imageField.ResolvedValue);
            Assert.AreEqual(0, imageField.Diagnostics.Count);
        }
    }
}
