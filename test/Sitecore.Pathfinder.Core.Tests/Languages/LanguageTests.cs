// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;
using Sitecore.Pathfinder.Languages.Json;
using Sitecore.Pathfinder.Languages.Xml;
using Sitecore.Pathfinder.Languages.Yaml;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Languages
{
    [TestFixture]
    public class LanguageTests : Tests
    {
        [NotNull]
        public IProject Project { get; set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start(GoodWebsite);
        }

        [Test]
        public void DiagnosticsTests()
        {
            Assert.IsAssignableFrom<XmlLanguage>(Services.LanguageService.GetLanguageByExtension("item.xml"));
            Assert.IsAssignableFrom<YamlLanguage>(Services.LanguageService.GetLanguageByExtension("item.yaml"));
            Assert.IsAssignableFrom<JsonLanguage>(Services.LanguageService.GetLanguageByExtension("item.json"));
        }
    }
}
