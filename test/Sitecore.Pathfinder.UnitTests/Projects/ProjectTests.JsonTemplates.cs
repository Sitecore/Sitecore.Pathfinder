﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Projects
{
    public partial class ProjectTests
    {
        [TestMethod]
        public void JsonTemplateTest()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/client/Applications/SitecoreWorks/templates/Toolbars/ToolbarTabResource");
            Assert.IsNotNull(projectItem);

            var template = (Template)projectItem;

            Assert.AreEqual("ToolbarTabResource", template.ShortName);
            Assert.AreEqual("{1930BBEB-7805-471A-A3BE-4858AC7CF696}", template.BaseTemplates);
            Assert.AreEqual("Applications/16x16/about.png", template.Icon);
            Assert.AreEqual("Short help.", template.ShortHelp);
            Assert.AreEqual("Long help.", template.LongHelp);

            var standardValuesItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/client/Applications/SitecoreWorks/templates/Toolbars/ToolbarTabResource/__Standard Values") as Item;
            Assert.IsNotNull(standardValuesItem);
            Assert.AreEqual(template.StandardValuesItem, standardValuesItem);

            var templateSection = template.Sections.FirstOrDefault(s => s.SectionName == "Fields");
            Assert.IsNotNull(templateSection);
            Assert.IsNotNull("Fields", templateSection.SectionName);

            var templateField = templateSection.Fields.FirstOrDefault(f => f.FieldName == "Text");
            Assert.IsNotNull(templateField);
            Assert.IsNotNull("Text", templateField.FieldName);
            Assert.IsNotNull("Single-Line Text", templateField.Type);
            Assert.IsTrue(templateField.Unversioned);
            Assert.IsFalse(templateField.Shared);
            Assert.AreEqual(0, templateField.Sortorder);
        }
    }
}
