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
            Start(BadWebsite);
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
            Assert.AreEqual(3, diagnostic.Span.LineNumber);
            Assert.AreEqual(10, diagnostic.Span.LinePosition);
            Assert.AreEqual(5, diagnostic.Span.LineLength);

            // link field
            var linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("/sitecore/media library/badlink", linkField.Value);
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
            diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Link field reference not found: /sitecore/media library/badlink");
            Assert.IsNotNull(diagnostic);

            // link field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink1");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("/sitecore/media library/badlink", linkField.Value);
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);

            // link field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink2");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("", linkField.Value);
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
          
            // multiple links field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "Multiple");
            Assert.IsNotNull(linkField);
            Assert.AreEqual("/sitecore/media library/badlink|/sitecore/media library/badlink", linkField.Value);
            Assert.AreEqual(string.Empty, linkField.ResolvedValue);
          
            // image field
            var imageField = item.Fields.FirstOrDefault(f => f.FieldName == "BadImage");
            Assert.IsNotNull(imageField);
            Assert.AreEqual("/sitecore/media library/badlink", imageField.Value);
            Assert.AreEqual(string.Empty, imageField.ResolvedValue);
            diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Image reference not found: /sitecore/media library/badlink");
            Assert.IsNotNull(diagnostic);

            // image field
            imageField = item.Fields.FirstOrDefault(f => f.FieldName == "BadImage2");
            Assert.IsNotNull(imageField);
            Assert.AreEqual(string.Empty, imageField.Value);
            Assert.AreEqual(string.Empty, imageField.ResolvedValue);
        }
    }
}
