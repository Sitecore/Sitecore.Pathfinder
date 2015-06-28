using System;
using System.IO;
using NUnit.Framework;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Data.Items;
using Sitecore.IO;

namespace Sitecore.Pathfinder.Tests
{
    [TestFixture]
    public class PathfinderTests
    {
        [Test]
        public void Test000_Layout_Webconfig_File()
        {
            Assert.IsTrue(File.Exists(FileUtil.MapPath("/layout/web.config")));
        }

        [Test]
        public void Test001_Layout_Renderings_Footerhtml_File()
        {
            Assert.IsTrue(File.Exists(FileUtil.MapPath("/layout/renderings/footer.html")));
        }

        [Test]
        public void Test002_Layout_Renderings_Htmltemplatehtml_File()
        {
            Assert.IsTrue(File.Exists(FileUtil.MapPath("/layout/renderings/htmltemplate.html")));
        }

        [Test]
        public void Test003_Sitecore_Templates_MyTemplate_Template()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/templates/MyTemplate");
            Assert.IsNotNull(item);
            var template = TemplateManager.GetTemplate(item.ID, database);
            Assert.IsNotNull(template);
            Assert.AreEqual("MyTemplate", template.Name);
            Assert.AreEqual("{1930BBEB-7805-471A-A3BE-4858AC7CF696}", item[FieldIDs.BaseTemplate]);

            var standardValuesItem = database.GetItem(item[FieldIDs.BaseTemplate]);
            Assert.IsNotNull(standardValuesItem);

            Item sectionItem;
            Item fieldItem;

            sectionItem = item.Children["Data"];
            Assert.IsNotNull(sectionItem);
            Assert.AreEqual("Data", sectionItem.Name);

            fieldItem = sectionItem.Children["Title"];
            Assert.IsNotNull(fieldItem);
            Assert.AreEqual("Title", fieldItem.Name);
            Assert.AreEqual("Rich Text", fieldItem[TemplateFieldIDs.Type]);
        }

        [Test]
        public void Test004_Sitecore_Content_Home_HtmlTemplate_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/content/Home/HtmlTemplate");
            Assert.IsNotNull(item);
            Assert.AreEqual("HtmlTemplate", item.Name);
            Assert.AreEqual("/sitecore/templates/Sample/Sample Item", database.GetItem(item.TemplateID).Paths.Path);
            Assert.AreEqual("Welcome", item["Title"]);
            Assert.AreEqual("Welcome to Sitecore", item["Text"]);
        }

        [Test]
        public void Test005_Sitecore_Layout_Renderings_Footer_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/layout/renderings/footer");
            Assert.IsNotNull(item);
            Assert.AreEqual("footer", item.Name);
            Assert.AreEqual("{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}", item.TemplateID.ToString());
            Assert.AreEqual("/layout/renderings/footer.html", item["Path"]);
        }

        [Test]
        public void Test006_Sitecore_Layout_Renderings_Htmltemplate_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/layout/renderings/htmltemplate");
            Assert.IsNotNull(item);
            Assert.AreEqual("htmltemplate", item.Name);
            Assert.AreEqual("{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}", item.TemplateID.ToString());
            Assert.AreEqual("/layout/renderings/htmltemplate.html", item["Path"]);
        }

    }
}
