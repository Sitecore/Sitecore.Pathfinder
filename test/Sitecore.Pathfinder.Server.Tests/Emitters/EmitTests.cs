// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Reflection;
using Sitecore.Pathfinder.Emitters;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Server.Tests.Emitters
{
    // [TestFixture]
    public class EmitTests
    {
        public const string GoodWebsite = "..\\..\\Website.Good\\bin";

        // [Test]
        public void ProjectTests()
        {
            try
            {
                Context.IsUnitTesting = true;
            }
            catch 
            {
            }

            var projectDirectory = PathHelper.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, GoodWebsite);
            var toolsDirectory = Path.Combine(projectDirectory, "sitecore.tools");
            var emitService = new EmitService(toolsDirectory, projectDirectory);

            emitService.Start();
        }
    }
}
