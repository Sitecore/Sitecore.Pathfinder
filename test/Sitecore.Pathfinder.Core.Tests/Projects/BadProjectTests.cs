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
            var item = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/XmlItem") as Item;
            NUnit.Framework.Assert.IsNotNull(item);

            var diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Item path reference not found: /sitecore/media library/badlink");
            NUnit.Framework.Assert.IsNotNull(diagnostic);
            NUnit.Framework.Assert.AreEqual(Severity.Error, diagnostic.Severity);
            NUnit.Framework.Assert.AreEqual(3, diagnostic.Span.LineNumber);
            NUnit.Framework.Assert.AreEqual(10, diagnostic.Span.LinePosition);
            NUnit.Framework.Assert.AreEqual(5, diagnostic.Span.Length);

            // link field
            var linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink");
            NUnit.Framework.Assert.IsNotNull(linkField);
            NUnit.Framework.Assert.AreEqual("/sitecore/media library/badlink", linkField.Value);
            NUnit.Framework.Assert.AreEqual(string.Empty, linkField.CompiledValue);
            diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Link field reference not found: /sitecore/media library/badlink");
            NUnit.Framework.Assert.IsNotNull(diagnostic);

            // link field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink1");
            NUnit.Framework.Assert.IsNotNull(linkField);
            NUnit.Framework.Assert.AreEqual("/sitecore/media library/badlink", linkField.Value);
            NUnit.Framework.Assert.AreEqual(string.Empty, linkField.CompiledValue);

            // link field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "BadLink2");
            NUnit.Framework.Assert.IsNotNull(linkField);
            NUnit.Framework.Assert.AreEqual("", linkField.Value);
            NUnit.Framework.Assert.AreEqual(string.Empty, linkField.CompiledValue);

            // multiple links field
            linkField = item.Fields.FirstOrDefault(f => f.FieldName == "Multiple");
            NUnit.Framework.Assert.IsNotNull(linkField);
            NUnit.Framework.Assert.AreEqual("/sitecore/media library/badlink|/sitecore/media library/badlink", linkField.Value);
            NUnit.Framework.Assert.AreEqual(string.Empty, linkField.CompiledValue);

            // image field
            var imageField = item.Fields.FirstOrDefault(f => f.FieldName == "BadImage");
            NUnit.Framework.Assert.IsNotNull(imageField);
            NUnit.Framework.Assert.AreEqual("/sitecore/media library/badlink", imageField.Value);
            NUnit.Framework.Assert.AreEqual(string.Empty, imageField.CompiledValue);
            diagnostic = Project.Diagnostics.FirstOrDefault(d => d.Text == "Image reference not found: /sitecore/media library/badlink");
            NUnit.Framework.Assert.IsNotNull(diagnostic);

            // image field
            imageField = item.Fields.FirstOrDefault(f => f.FieldName == "BadImage2");
            NUnit.Framework.Assert.IsNotNull(imageField);
            NUnit.Framework.Assert.AreEqual(string.Empty, imageField.Value);
            NUnit.Framework.Assert.AreEqual(string.Empty, imageField.CompiledValue);
        }
    }
}
