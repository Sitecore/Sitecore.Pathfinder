// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    using Sitecore.Pathfinder.Documents.Json;
    using Sitecore.Pathfinder.Documents.Xml;

    [TestFixture]
    public class ProjectTests : Tests
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
            Assert.AreEqual(8, Project.Options.ExternalReferences.Count);
            Assert.AreEqual("/sitecore/templates/Sample/Sample Item", Project.Options.ExternalReferences.ElementAt(0));
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
            Assert.AreEqual("JsonContentItem", item.ItemName.Value);
            Assert.AreEqual("/sitecore/content/Home/JsonContentItem", item.ItemIdOrPath);
            Assert.AreEqual("Sample Item", item.TemplateIdOrPath.Value);
            Assert.IsNotNull(item.ItemName.Source);
            Assert.AreEqual(typeof(FileNameTextNode), item.ItemName.Source.GetType());
            Assert.IsNotNull(item.TemplateIdOrPath.Source);
            Assert.AreEqual(typeof(JsonTextNode), item.TemplateIdOrPath.Source.GetType());
            Assert.AreEqual("Sample Item", item.TemplateIdOrPath.Source.Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName.Value == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello World", field.Value.Value);

            var textDocument = projectItem.Snapshot as ITextSnapshot;
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
            Assert.AreEqual("Foo", item.ItemName.Value);
            Assert.AreEqual("/sitecore/content/Home/Foo", item.ItemIdOrPath);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath.Value);
            Assert.IsNotNull(item.ItemName.Source);
            Assert.AreEqual(typeof(FileNameTextNode), item.ItemName.Source.GetType());

            var textDocument = projectItem.Snapshot as ITextSnapshot;
            Assert.IsNotNull(textDocument);

            var treeNode = textDocument.Root;
            Assert.AreEqual("Item", treeNode.Name);
            Assert.AreEqual(1, treeNode.Attributes.Count());

            var attr = treeNode.Attributes.First();
            Assert.AreEqual("Template.Create", attr.Name);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName.Value == "Title");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello", field.Value.Value);
        }

        [Test]
        public void JsonTemplateTest()
        {
            var projectItem = Project.Items.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/FooTemplate");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("FooTemplate", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/FooTemplate", projectItem.QualifiedName);

            var template = projectItem as Template;
            Assert.IsNotNull(template);
            Assert.AreEqual("FooTemplate", template.ItemName.Value);
            Assert.AreEqual("Applications/16x16/About.png", template.Icon);
            Assert.AreEqual("ShortHelp", template.ShortHelp);
            Assert.AreEqual("LongHelp", template.LongHelp);
            Assert.AreEqual("/sitecore/content/Home/FooTemplate", template.ItemIdOrPath);
            Assert.AreEqual(1, template.Sections.Count);
            Assert.IsNotNull(template.ItemName.Source);
            Assert.AreEqual(typeof(FileNameTextNode), template.ItemName.Source.GetType());

            var section = template.Sections[0];
            Assert.AreEqual("Fields", section.Name);
            Assert.AreEqual("Applications/16x16/About.png", section.Icon);
            Assert.AreEqual(2, section.Fields.Count);

            var field = section.Fields[0];
            Assert.AreEqual("Title", field.Name);
            Assert.AreEqual("Single-Line Text", field.Type);
            Assert.AreEqual("ShortHelp", field.ShortHelp);
            Assert.AreEqual("LongHelp", field.LongHelp);
            Assert.AreEqual("StandardValue", field.StandardValue);

            Assert.AreEqual("Rich Text", section.Fields[1].Type);
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

            var projectItem1 = new Item(project, "SameId", TextNode.Empty, string.Empty, "SameId", string.Empty, string.Empty);
            var projectItem2 = new Item(project, "SameId", TextNode.Empty, string.Empty, "SameId", string.Empty, string.Empty);

            project.AddOrMerge(projectItem1);
            project.AddOrMerge(projectItem2);

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
            Assert.AreEqual("Mushrooms", item.ItemName.Value);
            Assert.AreEqual("/sitecore/media library/Mushrooms", item.ItemIdOrPath);

            var field = item.Fields.FirstOrDefault(f => f.FieldName.Value == "Description");
            Assert.IsNotNull(field);
            Assert.AreEqual("Mushrooms", field.Value.Value);
        }

        [Test]
        public void RemapFileDirectoryTests()
        {
            Assert.AreEqual(1, Project.Options.RemapFileDirectories.Count);
            Assert.AreEqual("/sitecore/shell/client", Project.Options.RemapFileDirectories["/sitecore/client"]);
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
            Assert.AreEqual("Pipper", item.ItemName.Value);
            Assert.AreEqual("/sitecore/content/Home/Pipper", item.ItemIdOrPath);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", item.TemplateIdOrPath.Value);

            var field = item.Fields.FirstOrDefault();
            Assert.IsNotNull(field);
            Assert.AreEqual("__Workflow", field.FieldName.Value);
            Assert.AreEqual("{A5BC37E7-ED96-4C1E-8590-A26E64DB55EA}", field.Value.Value);

            field = item.Fields.ElementAt(1);
            Assert.IsNotNull(field);
            Assert.AreEqual("Title", field.FieldName.Value);
            Assert.AreEqual("Pip 1", field.Value.Value);
            Assert.AreEqual("en", field.Language.Value);
            Assert.AreEqual(1, field.Version.Value);
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
            Assert.AreEqual("XmlContentItem", item.ItemName.Value);
            Assert.AreEqual("/sitecore/content/XmlContentItem", item.ItemIdOrPath);
            Assert.AreEqual("Sample-Item", item.TemplateIdOrPath.Value);
            Assert.IsNotNull(item.ItemName.Source);
            Assert.AreEqual(typeof(FileNameTextNode), item.ItemName.Source.GetType());
            Assert.AreEqual(typeof(XmlTextNode), item.TemplateIdOrPath.Source?.GetType());
            Assert.AreEqual("Sample-Item", item.TemplateIdOrPath.Source?.Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName.Value == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello World", field.Value.Value);
            Assert.AreEqual(typeof(XmlTextNode), field.Value.Source?.GetType());
            Assert.AreEqual("Hello World", field.Value.Source?.Value);
            Assert.AreEqual("Text", field.Value.Source?.Name);

            var textDocument = projectItem.Snapshot as ITextSnapshot;
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
            Assert.AreEqual("HelloWorld", item.ItemName.Value);
            Assert.AreEqual("/sitecore/content/Home/HelloWorld", item.ItemIdOrPath);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath.Value);
            Assert.IsNotNull(item.ItemName.Source);
            Assert.AreEqual(typeof(FileNameTextNode), item.ItemName.Source.GetType());
            Assert.AreEqual(typeof(XmlTextNode), item.TemplateIdOrPath.Source?.GetType());
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", item.TemplateIdOrPath.Source?.Value);
            Assert.AreEqual("Template.Create", item.TemplateIdOrPath.Source?.Name);

            var field = item.Fields.FirstOrDefault(f => f.FieldName.Value == "Title");
            Assert.IsNotNull(field);
            Assert.AreEqual("Hello", field.Value.Value);

            var textDocument = projectItem.Snapshot as ITextSnapshot;
            Assert.IsNotNull(textDocument);

            var treeNode = textDocument.Root;
            Assert.AreEqual("Item", treeNode.Name);
            Assert.AreEqual(1, treeNode.Attributes.Count());

            var attr = treeNode.Attributes.First();
            Assert.AreEqual("Template.Create", attr.Name);
            Assert.AreEqual("/sitecore/templates/Sample/HelloWorld", attr.Value);
        }
    }
}
