// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests : Tests
    {
        [NotNull]
        public IProject Project { get; set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start(GoodWebsite);

            Project = Services.ProjectService.LoadProjectFromConfiguration();

            foreach (var diagnostic in Project.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Severity} ({diagnostic.Span.LineNumber}, {diagnostic.Span.LinePosition}, {diagnostic.Span.Length}): {diagnostic.Text} [{diagnostic.FileName}]");
                Console.WriteLine();
            }
        }

        [Test]
        public void AddRemoveTests()
        {
            var project = Resolve<IProject>().Load(new ProjectOptions(ProjectDirectory, "master"), Enumerable.Empty<string>());
            var count = project.SourceFiles.Count;

            var fileName = Path.Combine(ProjectDirectory, "content\\Home\\XmlItem.item.xml");

            project.Add(fileName);
            Assert.AreEqual(count + 1, project.SourceFiles.Count);
            Assert.AreEqual(fileName, project.SourceFiles.Last().AbsoluteFileName);

            project.Remove(fileName);
            Assert.AreEqual(count, project.SourceFiles.Count);
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

        [Test]
        public void FindUsagesTest()
        {
            var references = Services.QueryService.FindUsages(Project, "/sitecore/media library/mushrooms").ToList();
            Assert.AreEqual(9, references.Count);
        }

        [Test]
        public void LoadProjectTests()
        {
            Assert.IsTrue(Project.ProjectItems.Any());
            Assert.IsTrue(Project.SourceFiles.Any());
        }

        [Test]
        public void MergeByProjectUniqueIdTest()
        {
            var project = Resolve<IProject>();
            var context = Services.CompositionService.Resolve<IParseContext>().With(project, Snapshot.Empty, PathMappingContext.Empty);

            var sameGuid = new Guid("{11CDDC59-0F73-4A6E-90E2-6614418F173E}");
            var projectItem1 = new Item(project, TextNode.Empty, sameGuid, string.Empty, "SameId", string.Empty, string.Empty);
            var projectItem2 = new Item(project, TextNode.Empty, sameGuid, string.Empty, "SameId", string.Empty, string.Empty);

            project.AddOrMerge(projectItem1);
            project.AddOrMerge(projectItem2);

            Assert.AreEqual(1, project.ProjectItems.Count());
        }

        [Test]
        public void MergeTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/media library/Mushrooms");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("Mushrooms", projectItem.ShortName);
            Assert.AreEqual("/sitecore/media library/Mushrooms", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("Mushrooms", item.ItemName);
            Assert.AreEqual("/sitecore/media library/Mushrooms", item.ItemIdOrPath);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Description");
            Assert.IsNotNull(field);
            Assert.AreEqual("Mushrooms", field.Value);
        }

        [Test]
        public void SerializationItemTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/SerializedItem");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("SerializedItem", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/SerializedItem", projectItem.QualifiedName);
            Assert.AreEqual("{CEABE4B1-E915-4904-B396-BBC0C081F111}", projectItem.Uri.Guid.Format());
            Assert.AreEqual(1, projectItem.Snapshots.Count);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("SerializedItem", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/SerializedItem", item.ItemIdOrPath);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", item.TemplateIdOrPath);

            var field = item.Fields.FirstOrDefault();
            Assert.IsNotNull(field);
            Assert.AreEqual("__Workflow", field.FieldName);
            Assert.AreEqual("{A5BC37E7-ED96-4C1E-8590-A26E64DB55EA}", field.Value);

            field = item.Fields.ElementAt(1);
            Assert.IsNotNull(field);
            Assert.AreEqual("Title", field.FieldName);
            Assert.AreEqual("Pip 1", field.Value);
            Assert.AreEqual("Pip 1", field.CompiledValue);
            Assert.AreEqual("en", field.Language);
            Assert.AreEqual(1, field.Version);
        }

        [Test]
        public void TemplateMergeTest()
        {
            var template = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/templates/Xml-Template-Merge") as Template;
            Assert.IsNotNull(template);

            Assert.AreEqual(2, template.Sections.Count);
            Assert.AreEqual("Fields", template.Sections.First().SectionName);
            Assert.AreEqual(3, template.Sections.First().Fields.Count);
            Assert.AreEqual("Data", template.Sections.ElementAt(1).SectionName);
            Assert.AreEqual(2, template.Sections.Last().Fields.Count);
        }
    }
}
