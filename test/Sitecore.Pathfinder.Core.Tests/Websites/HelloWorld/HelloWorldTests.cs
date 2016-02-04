using System;
using NUnit.Framework;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Websites.HelloWorld
{
    [TestFixture]
    public class HelloWorldTests : Tests
    {
        [Diagnostics.NotNull]
        public IProject Project { get; private set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start("HelloWorld");

            Project = Services.ProjectService.LoadProjectFromConfiguration();

            foreach (var diagnostic in Project.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Severity} ({diagnostic.Span.LineNumber}, {diagnostic.Span.LinePosition}, {diagnostic.Span.Length}): {diagnostic.Text} [{diagnostic.FileName}]");
                Console.WriteLine();
            }
        }

        [Test]
        public void DiagnosticsTests()
        {
            foreach (var diagnostic in Project.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Severity} ({diagnostic.Span.LineNumber}, {diagnostic.Span.LinePosition}, {diagnostic.Span.Length}): {diagnostic.Text} [{diagnostic.FileName}]");
                Console.WriteLine();
            }

            Assert.AreEqual(0, Project.Diagnostics.Count);
        }
    }
}