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
        public void Test0_sitecorecontentHomeHelloWorld()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/content/Home/HelloWorld");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test1_sitecorecontentHomeWelcome()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/content/Home/Welcome");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test2_sitecorelayoutrenderingsHelloWorld()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/layout/renderings/HelloWorld");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test3_sitecoremediaLibrarylighthouse()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/media library/lighthouse");
            Assert.IsNotNull(item);
        }

    }
}
