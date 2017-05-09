// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Configuration
{
    [TestClass]
    public class ConfigurationServicesTests
    {
        [TestMethod]
        public void AddCommandLineTest()
        {
            var configuration = new ConfigurationModel.Configuration();
            configuration.Add(new MemoryConfigurationSource());
            var configurationService = new ConfigurationService(configuration);

            IEnumerable<string> args = new List<string>()
            {
              "build", "/param1=1", "/param2=2 2", "/param3", "3", "posarg1", "--switch1", "true", "/param4", "4 4"
            };

            configurationService.AddCommandLine(configuration, args);

            Assert.AreEqual("build", configuration.GetString("arg0"));
            Assert.AreEqual("posarg1", configuration.GetString("arg1"));

            Assert.AreEqual("1", configuration.GetString("param1"));
            Assert.AreEqual("2 2", configuration.GetString("param2"));
            Assert.AreEqual("3", configuration.GetString("param3"));
            Assert.AreEqual("4 4", configuration.GetString("param4"));

            Assert.AreEqual("true", configuration.GetString("switch1"));
        }
    }
}
