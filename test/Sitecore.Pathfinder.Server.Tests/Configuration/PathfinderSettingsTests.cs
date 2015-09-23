// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;
using Sitecore.Pathfinder.Configuration;

namespace Sitecore.Pathfinder.Server.Tests.Configuration
{
    [TestFixture]
    public class PathfinderSettingsTests
    {
        [Test]
        public void SettingsTests()
        {
            Assert.AreEqual(".html", PathfinderSettings.HtmlTemplateExtension);
        }
    }
}
