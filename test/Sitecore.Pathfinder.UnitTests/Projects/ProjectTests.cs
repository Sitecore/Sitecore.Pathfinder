// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    [TestClass]
    public partial class ProjectTests : Tests
    {
        [Diagnostics.NotNull]
        public IProject Project { get; private set; }

        [TestInitialize]
        public void Startup()
        {
            Start();

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

            // Assert.AreEqual(2, Project.Diagnostics.Count);
        }

        [TestMethod]
        public void LoadProjectTests()
        {
            Assert.IsTrue(Project.ProjectItems.Any());
        }

        [TestMethod]
        public void MergeByProjectUniqueIdTest()
        {
            var project = Services.CompositionService.Resolve<IProject>();

            var sameGuid = new Guid("{11CDDC59-0F73-4A6E-90E2-6614418F173E}");
            var projectItem1 = new Item(project, sameGuid, string.Empty, "SameId", string.Empty, string.Empty);
            var projectItem2 = new Item(project, sameGuid, string.Empty, "SameId", string.Empty, string.Empty);

            project.AddOrMerge(projectItem1);
            project.AddOrMerge(projectItem2);

            Assert.AreEqual(1, project.ProjectItems.Count());
        }

        [TestMethod]
        public void SerializationItemTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Serialization/Home");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("Home", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Serialization/Home", projectItem.QualifiedName);
            Assert.AreEqual("{CEABE4B1-E915-4904-B396-BBC0C081F111}", projectItem.Uri.Guid.Format());
            Assert.AreEqual(1, projectItem.GetSnapshots().Count());

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("Home", item.ItemName);
            Assert.AreEqual("/sitecore/content/Serialization/Home", item.ItemIdOrPath);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", item.TemplateIdOrPath);

            var field = item.Fields.FirstOrDefault();
            Assert.IsNotNull(field);
            Assert.AreEqual("Title", field.FieldName);
            Assert.AreEqual("Pip 1", field.Value);

            field = item.Fields.ElementAt(1);
            Assert.IsNotNull(field);
            Assert.AreEqual("Text", field.FieldName);
            Assert.AreEqual("Pip 2", field.Value);
            Assert.AreEqual("Pip 2", field.CompiledValue);
            Assert.AreEqual("en", field.Language.LanguageName);
            Assert.AreEqual(1, field.Version.Number);
        }
    }
}
