// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests
    {
        [Test]
        public void XmlTransformTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/XmlTransform");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("XmlTransform", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/XmlTransform", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("XmlTransform", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/XmlTransform", item.ItemIdOrPath);
            Assert.AreEqual("/sitecore/templates/Sample/XmlTransformTemplate", item.TemplateIdOrPath);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Lorem Ipsum", field.Value);
        }
    }
}
