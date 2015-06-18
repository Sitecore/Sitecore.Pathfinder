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
        public void Test1_sitecorecontentHomeHola()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/content/Home/Hola");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test2_sitecorecontentHomeWelcome()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/content/Home/Welcome");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test3_sitecorelayoutrenderingsHelloWorld()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/layout/renderings/HelloWorld");
            Assert.IsNotNull(item);
        }

        [Test]
        public void Test4_sitecoremediaLibrarydemo()
        {
            var item = Factory.GetDatabase("master").GetItem("/sitecore/media library/demo");
            
            
            
            Assert.IsNotNull(item);
        }

    }
}
