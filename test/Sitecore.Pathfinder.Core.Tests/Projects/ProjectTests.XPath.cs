// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Xml.XPath;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests
    {
        private object Evaluate([NotNull] string query)
        {
            var context = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/Home/XmlItem");

            var q = new XPathExpression(Services.XPathService);
            q.Parse(query);
            return q.Evaluate(context);
        }

        [Test]
        public void QueryTests()
        {
            Assert.AreEqual(1, Evaluate("1"));
            Assert.AreNotEqual(1, Evaluate("2"));
            Assert.AreEqual(2, Evaluate("1 + 1"));
            Assert.AreEqual("XmlItem", Evaluate("@@name"));
            Assert.AreEqual(true, Evaluate("@@name = 'XmlItem'"));
            Assert.AreEqual("Hello World", Evaluate("@Text"));
            Assert.AreEqual(true, Evaluate("@Text = 'Hello World'"));
            Assert.IsTrue(Evaluate(".").GetType() == typeof(Item));
            Assert.AreEqual("XmlItemTemplate", Evaluate("@@templatename"));
            Assert.AreEqual("XmlItemTemplate123", Evaluate("@@templatename + '123'"));
            Assert.AreEqual(2, ((object[])Evaluate(". | .")).Length);
            Assert.AreEqual(true, Evaluate("startswith('Hello World', 'Hello')"));
            Assert.AreEqual(true, Evaluate("not(false)"));
            Assert.AreEqual("XmlItem", Evaluate("../../Home/XmlItem/@@name"));
        }
    }
}
