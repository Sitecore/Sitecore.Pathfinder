// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Languages.Serialization;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests
    {
        [Test]
        public void SerializedItemTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/SerializedItem");
            Assert.IsNotNull(projectItem);
            Assert.AreEqual("SerializedItem", projectItem.ShortName);
            Assert.AreEqual("/sitecore/content/Home/SerializedItem", projectItem.QualifiedName);

            var item = projectItem as Item;
            Assert.IsNotNull(item);
            Assert.AreEqual("SerializedItem", item.ItemName);
            Assert.AreEqual("/sitecore/content/Home/SerializedItem", item.ItemIdOrPath);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", item.TemplateIdOrPath);
            Assert.IsNotNull(item.ItemNameProperty.SourceTextNodes);
            Assert.IsInstanceOf<TextNode>(item.ItemNameProperty.SourceTextNode);
            Assert.IsInstanceOf<TextNode>(item.TemplateIdOrPathProperty.SourceTextNode);
            Assert.AreEqual("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", TraceHelper.GetTextNode(item.TemplateIdOrPathProperty).Value);

            var field = item.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(field);
            Assert.AreEqual("Pip 2", field.Value);
            Assert.IsInstanceOf<TextNode>(field.ValueProperty.SourceTextNode);
            Assert.AreEqual("Pip 2", field.ValueProperty.SourceTextNode?.Value);

            var textDocument = projectItem.Snapshots.First() as ITextSnapshot;
            Assert.IsNotNull(textDocument);
        }

        [Test]
        public void WriteSerializedItemTest()
        {
            var item = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/SerializedItem") as Item;
            Assert.IsNotNull(item);

            var writer = new StringWriter();
            item.WriteAsSerialization(writer);
            var result = writer.ToString();

            var expected = @"----item----
version: 1
id: {CEABE4B1-E915-4904-B396-BBC0C081F111}
database: master
path: /sitecore/content/Home/SerializedItem
parent: {110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}
name: SerializedItem
master: {00000000-0000-0000-0000-000000000000}
template: {76036F5E-CBCE-46D1-AF0A-4143F9B557AA}
templatekey: Sample Item

----field----
field: {A4F985D9-98B3-4B52-AAAF-4344F6E747C6}
name: __Workflow
key: __workflow
content-length: 38

{A5BC37E7-ED96-4C1E-8590-A26E64DB55EA}
----version----
language: en
version: 1
revision: 

----field----
field: {75577384-3C97-45DA-A847-81B00500E250}
name: Title
key: title
content-length: 5

Pip 1
----field----
field: {A60ACD61-A6DB-4182-8329-C957982CEC74}
name: Text
key: text
content-length: 5

Pip 2
----field----
field: {25BED78C-4957-4165-998A-CA1B52F67497}
name: __Created
key: __created
content-length: 16

20150602T111618Z
----field----
field: {5DD74568-4D4B-44C1-B513-0AF5F4CDA34F}
name: __Created by
key: __created by
content-length: 14

sitecore\admin
----field----
field: {8CDC337E-A112-42FB-BBB4-4143751E123F}
name: __Revision
key: __revision
content-length: 36

9ac1cdd2-40d5-466e-b3d6-00797bfc0e62
----field----
field: {D9CF14B1-FA16-4BA6-9288-E8A174D4D522}
name: __Updated
key: __updated
content-length: 35

20150602T111625:635688405856685957Z
----field----
field: {BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}
name: __Updated by
key: __updated by
content-length: 14

sitecore\admin
----field----
field: {3E431DE1-525E-47A3-B6B0-1CCBEC3A8C98}
name: __Workflow state
key: __workflow state
content-length: 38

{190B1C84-F1BE-47ED-AA41-F42193D9C8FC}
";
            Assert.AreEqual(expected, result);
        }
    }
}
