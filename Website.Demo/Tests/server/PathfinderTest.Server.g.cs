using System;
using System.IO;
using NUnit.Framework;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.IO;

namespace Sitecore.Pathfinder.Tests
{
    [TestFixture]
    public class PathfinderTests
    {
        [Test]
        public void Test000_Sitecore_Content_Home_HelloWorld_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/content/Home/HelloWorld");
            Assert.IsNotNull(item);
            Assert.AreEqual("HelloWorld", item.Name);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", database.GetItem(item.TemplateID).Paths.Path);
            Assert.AreEqual("Welcome", item["Title"]);
            Assert.AreEqual("Welcome to Sitecore", item["Text"]);
            Assert.AreEqual("/sitecore/media library/demo", item["Image"]);
        }

        [Test]
        public void Test001_Sitecore_Content_Home_Welcome_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/content/Home/Welcome");
            Assert.IsNotNull(item);
            Assert.AreEqual("Welcome", item.Name);
            Assert.AreEqual("/sitecore/templates/Sample/Sample Item", database.GetItem(item.TemplateID).Paths.Path);
            Assert.AreEqual("Pathfinder Demo", item["Title"]);
            Assert.AreEqual("Welcome to Sitecore Pathfinder demo", item["Text"]);
            Assert.AreEqual("", item["__Workflow"]);
            Assert.AreEqual("", item["__Workflow State"]);
        }

        [Test]
        public void Test002_Sitecore_Layout_Renderings_HelloWorld_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/layout/renderings/HelloWorld");
            Assert.IsNotNull(item);
            Assert.AreEqual("HelloWorld", item.Name);
            Assert.AreEqual("{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}", item.TemplateID.ToString());
            Assert.AreEqual("/layout/renderings/HelloWorld.cshtml", item["Path"]);
        }

        [Test]
        public void Test003_Sitecore_MediaLibrary_Demo_Item()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/media library/demo");
            Assert.IsNotNull(item);
            Assert.AreEqual("demo", item.Name);
            Assert.AreEqual("Light demo", item["Description"]);
        }

        [Test]
        public void Test004_Sitecore_Templates_Sample_HelloWorld_Template()
        {
            var database = Factory.GetDatabase("master");
            var item = database.GetItem("/sitecore/templates/Sample/HelloWorld");
            Assert.IsNotNull(item);
            var template = TemplateManager.GetTemplate(item.ID, database);
            Assert.IsNotNull(template);
            Assert.AreEqual("HelloWorld", template.Name);
        }

        [Test]
        public void Test005_Layout_Webconfig_File()
        {
            Assert.IsTrue(File.Exists(FileUtil.MapPath("/layout/web.config")));
        }

        [Test]
        public void Test006_Layout_Renderings_HelloWorldcshtml_File()
        {
            Assert.IsTrue(File.Exists(FileUtil.MapPath("/layout/renderings/HelloWorld.cshtml")));
        }

    }
}
