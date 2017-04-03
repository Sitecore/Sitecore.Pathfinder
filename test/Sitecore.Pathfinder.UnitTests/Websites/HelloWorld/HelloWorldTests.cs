using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Websites.HelloWorld
{
    [TestClass]
    public class HelloWorldTests : Tests
    {
        [Diagnostics.NotNull]
        public IProject Project { get; private set; }

        [TestInitialize]
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

        [TestMethod]
        public void DiagnosticsTests()
        {
            foreach (var diagnostic in Project.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Severity} ({diagnostic.Span.LineNumber}, {diagnostic.Span.LinePosition}, {diagnostic.Span.Length}): {diagnostic.Text} [{diagnostic.FileName}]");
                Console.WriteLine();
            }

            Assert.AreEqual(1, Project.Diagnostics.Count());
        }
    }
}