// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Unicorn.Languages.Unicorn;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests
    {
        [Test]
        public void UnicornItemTest()
        {
            var unicornFile = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/YmlItem") as UnicornFile;

            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/YmlItem");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("YmlItem", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/YmlItem", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("YmlItem", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/YmlItem", item.ItemIdOrPath);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", item.TemplateIdOrPath);
            Assert.AreEqual("Sample Item", item.Template.ShortName);
        }
    }
}
