using System;
using NUnit.Framework;
using System.IO;
using Sitecore;
using Sitecore.IO;
using Sitecore.Configuration;

namespace Sitecore.Pathfinder.Tests
{
    [TestFixture]
    public class MyTest
    {
        [Test]
        public void MyTest0001()
        {
            Assert.IsTrue(File.Exists(FileUtil.MapPath("/layout/renderings/HelloWorld.cshtml")));
        }

    }
}
