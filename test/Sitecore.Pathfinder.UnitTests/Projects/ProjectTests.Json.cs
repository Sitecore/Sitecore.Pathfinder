// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Languages.Json;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    public partial class ProjectTests
    {
        [TestMethod]
        public void JsonItemTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/client/Applications/SitecoreWorks/content/content-editor/toolbar1/Home/Write/Save");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("Save", projectItem.ShortName);
            Assert.AreEqual("/sitecore/client/Applications/SitecoreWorks/content/content-editor/toolbar1/Home/Write/Save", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("Save", item.ItemName);
            Assert.AreEqual("/sitecore/client/Applications/SitecoreWorks/content/content-editor/toolbar1/Home/Write/Save", item.ItemIdOrPath);
            Assert.AreEqual("ToolbarLargeButtonResource", item.TemplateIdOrPath);
            Assert.AreNotEqual(item.ItemNameProperty.SourceTextNode, TextNode.Empty);
            Assert.AreEqual(0, item.ItemNameProperty.AdditionalSourceTextNodes.Count());

            // text field
            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Save", field.Value);
        }

        [TestMethod]
        public void WriteJsonItemTest()
        {
            var item = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/client/Applications/SitecoreWorks/content/content-editor/toolbar1/Home/Write/Save") as Item;
            Assert.IsNotNull(item);

            var writer = new StringWriter();

            item.WriteAsJson(writer);

            var result = writer.ToString();

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty((result)));
        }
    }
}
