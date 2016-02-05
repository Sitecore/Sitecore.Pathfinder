// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests
    {
        [Test]
        public void BuildPageHtml()
        {
            var projectItem = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/PageHtml");
            Assert.IsNotNull(projectItem);

            var item = projectItem as Item;
            Assert.IsNotNull(item);

            var layout = item.Fields.FirstOrDefault(f => f.FieldName == "__Renderings");
            Assert.IsNotNull(layout);

            var v = layout.CompiledValue;
            Console.WriteLine(v);
        }
    }
}
