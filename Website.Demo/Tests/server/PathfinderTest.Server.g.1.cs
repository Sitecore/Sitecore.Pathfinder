using System;
using NUnit.Framework;
using Sitecore;
using Sitecore.Configuration;

namespace Sitecore.Pathfinder.Tests
{
    [TestFixture]
    public class PathfinderTests
    {
        [Test]
        public void Test000_Sitecore_Content_Home_HelloWorld_Item()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/content/Home/HelloWorld");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test001_Sitecore_Content_Home_Welcome_Item()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/content/Home/Welcome");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test002_Sitecore_Layout_Renderings_HelloWorld_Item()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/layout/renderings/HelloWorld");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test003_Sitecore_MediaLibrary_Demo_Item()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/media library/demo");
            Assert.IsNotNull(item);
        }

    }
}
