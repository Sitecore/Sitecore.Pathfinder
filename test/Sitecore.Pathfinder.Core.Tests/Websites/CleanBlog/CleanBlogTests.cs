// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Projects;

namespace Sitecore.Pathfinder.Websites.CleanBlog
{
    [TestFixture]
    public class CleanBlogTests : Tests
    {
        [Diagnostics.NotNull]
        public IProject Project { get; private set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start("CleanBlog");

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

            Assert.AreEqual(1, Project.Diagnostics.Count());
        }
    }
}
