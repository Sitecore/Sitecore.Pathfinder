// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using NUnit.Framework;

namespace Sitecore.Pathfinder.Configuration
{
    [TestFixture]
    public class ConfigurationServicesTests
    {
        [Test]
        public void AddCommandLineTest()
        {
            var configuration = new Microsoft.Framework.ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());
            var configurationService = new ConfigurationService(configuration);

            IEnumerable<string> args = new List<string>()
            {
              "build", "/param1=1", "/param2=2 2", "/param3", "3", "posarg1", "--switch1", "true", "/param4", "4 4"
            };

            configurationService.AddCommandLine(configuration, args);

            Assert.AreEqual("build", configuration.Get("arg0"));
            Assert.AreEqual("posarg1", configuration.Get("arg1"));

            Assert.AreEqual("1", configuration.Get("param1"));
            Assert.AreEqual("2 2", configuration.Get("param2"));
            Assert.AreEqual("3", configuration.Get("param3"));
            Assert.AreEqual("4 4", configuration.Get("param4"));

            Assert.AreEqual("true", configuration.Get("switch1"));
        }
    }
}
