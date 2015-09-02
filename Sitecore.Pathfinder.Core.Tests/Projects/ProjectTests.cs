// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Snapshots.Xml;

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
            Start();
            Project = Services.ProjectService.LoadProjectFromConfiguration();
        }

        [Test]
        public void AddRemoveTests()
        {
            var project = Resolve<IProject>().Load(new ProjectOptions(ProjectDirectory, "master"), Enumerable.Empty<string>());

            var fileName = Path.Combine(ProjectDirectory, "content\\Home\\HelloWorld.item.xml");

            project.Add(fileName);
            Assert.AreEqual(1, project.SourceFiles.Count);
            Assert.AreEqual(fileName, project.SourceFiles.First().FileName);

            project.Remove(fileName);
            Assert.AreEqual(0, project.SourceFiles.Count);
        }

        [Test]
        public void ExternalReferencesTests()
        {
            Assert.AreEqual(4, Project.Options.ExternalReferences.Count);
            Assert.AreEqual("/sitecore/templates/Sample/Sample Item", Project.Options.ExternalReferences.ElementAt(0).Item2);
        }

        [Test]
        public void FindUsagesTest()
        {
            var references = Services.QueryService.FindUsages(Project, "/sitecore/media library/mushrooms");
            Assert.AreEqual(2, references.Count());
        }

        [Test]
        public void JsonContentItemTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/JsonContentItem");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("JsonContentItem", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/JsonContentItem", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("JsonContentItem", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/JsonContentItem", item.ItemIdOrPath);
            Assert.AreEqual("Sample Item", item.TemplateIdOrPath);
            Assert.IsNotNull(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<FileNameTextNode>(item.ItemNameProperty.SourceTextNodes);
            Assert.IsNotNull(item.TemplateIdOrPathProperty.SourceTextNodes);
            Assert.IsInstanceOf<AttributeNameTextNode>(item.TemplateIdOrPathProperty.SourceTextNodes);
            Assert.AreEqual("Sample Item", TraceHelper.FirstTextNode(item.TemplateIdOrPathProperty).Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello World", field.Value);

            var textDocument = projectItem.Snapshots.First() as ITextSnapshot;
            Assert.IsNotNull(textDocument);
        }

        [Test]
        public void JsonItemTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/Foo");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("Foo", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/Foo", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("Foo", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/Foo", item.ItemIdOrPath);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath);
            Assert.IsNotNull(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<FileNameTextNode>(item.ItemNameProperty.SourceTextNodes);

            var textDocument = projectItem.Snapshots.First() as ITextSnapshot;
            Assert.IsNotNull(textDocument);

            var treeNode = textDocument.Root;
            Assert.AreEqual("Item", treeNode.Name);
            Assert.AreEqual(2, treeNode.Attributes.Count());

            var attr = treeNode.Attributes.First();
            Assert.AreEqual("Template", attr.Name);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);
            attr = treeNode.Attributes.ElementAt(1);
            Assert.AreEqual("Template.CreateFromFields", attr.Name);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Title");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello", field.Value);
        }

        [Test]
        public void LoadProjectTests()
        {
            Assert.IsTrue(Project.Items.Any());
            Assert.IsTrue(Project.SourceFiles.Any());
        }

        [Test]
        public void MergeByProjectUniqueIdTest()
        {
            var project = Resolve<IProject>();
            var context = Services.CompositionService.Resolve<IParseContext>().With(project, Snapshot.Empty);

            var projectItem1 = new Item(project, "SameId", TextNode.Empty, string.Empty, "SameId", string.Empty, string.Empty);
            var projectItem2 = new Item(project, "SameId", TextNode.Empty, string.Empty, "SameId", string.Empty, string.Empty);

            project.AddOrMerge(context, projectItem1);
            project.AddOrMerge(context, projectItem2);

            Assert.AreEqual(1, project.Items.Count());
        }

        [Test]
        public void MergeTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/media library/Mushrooms");
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
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/Pipper");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("Pipper", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/Pipper", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("Pipper", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/Pipper", item.ItemIdOrPath);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", item.TemplateIdOrPath);

            var field = item.Fields.FirstOrDefault();
            Assert.IsNotNull(field);
            Assert.AreEqual("__Workflow", field.FieldName);
            Assert.AreEqual("{A5BC37E7-ED96-4C1E-8590-A26E64DB55EA}", field.Value);

            field = item.Fields.ElementAt(1);
            Assert.IsNotNull(field);
            Assert.AreEqual("Title", field.FieldName);
            Assert.AreEqual("Pip 1", field.Value);
            Assert.AreEqual("en", field.Language);
            Assert.AreEqual(1, field.Version);
        }

        [Test]
        public void XmlContentItemTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/XmlContentItem");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("XmlContentItem", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/XmlContentItem", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("XmlContentItem", item.ItemName);
            Assert.AreEqual("/sitecore/content/XmlContentItem", item.ItemIdOrPath);
            Assert.AreEqual("Sample-Item", item.TemplateIdOrPath);
            Assert.IsNotNull(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<FileNameTextNode>(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<AttributeNameTextNode>(item.TemplateIdOrPathProperty.SourceTextNodes);
            Assert.AreEqual("Sample-Item", TraceHelper.FirstTextNode(item.TemplateIdOrPathProperty).Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello World", field.Value);
            Assert.IsInstanceOf<XmlTextNode>(field.ValueProperty.SourceTextNodes);
            Assert.AreEqual("Hello World", field.ValueProperty.SourceTextNodes.First().Value);
            Assert.AreEqual("Text", field.ValueProperty.SourceTextNodes.First().Name);

            var textDocument = projectItem.Snapshots.First() as ITextSnapshot;
            Assert.IsNotNull(textDocument);

            var treeNode = textDocument.Root;
            Assert.AreEqual("Sample-Item", treeNode.Name);
            Assert.AreEqual(2, treeNode.Attributes.Count());
        }

        [Test]
        public void XmlItemTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/HelloWorld");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("HelloWorld", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/HelloWorld", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("HelloWorld", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/HelloWorld", item.ItemIdOrPath);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath);
            Assert.IsNotNull(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<FileNameTextNode>(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<XmlTextNode>(item.TemplateIdOrPathProperty.SourceTextNodes);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPathProperty.SourceTextNodes.First().Value);
            Assert.AreEqual("Template", item.TemplateIdOrPathProperty.SourceTextNodes.First().Name);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Title");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello", field.Value);

            var textDocument = projectItem.Snapshots.First() as ITextSnapshot;
            Assert.IsNotNull(textDocument);

            var treeNode = textDocument.Root;
            Assert.AreEqual("Item", treeNode.Name);
            Assert.AreEqual(2, treeNode.Attributes.Count());

            var attr = treeNode.Attributes.First();
            Assert.AreEqual("Template", attr.Name);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);
        }
    }
}
