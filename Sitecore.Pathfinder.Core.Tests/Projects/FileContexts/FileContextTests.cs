// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.ConfigurationModel;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Projects.FileContexts
{
    [TestFixture]
    public class FileContextTests : Tests
    {
        [NotNull]
        public IProject Project { get; set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start();
        }

        [Test]
        public void Tests()
        {
            var projectDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Projects\\FileContexts");

            Services.Configuration.AddJsonFile(projectDirectory + "\\scconfig.json");

            var files = Directory.GetFiles(projectDirectory, "*", SearchOption.AllDirectories);

            var project = Resolve<IProject>().Load(new ProjectOptions(projectDirectory, "master"), files);

            var item = project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/media library/mushrooms");
            Assert.IsNotNull(item);
        }
    }
}
