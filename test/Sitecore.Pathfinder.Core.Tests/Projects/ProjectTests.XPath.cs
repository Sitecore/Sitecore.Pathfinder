// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using NUnit.Framework;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.XPath;

namespace Sitecore.Pathfinder.Projects
{
    [TestFixture]
    public partial class ProjectTests
    {
        [CanBeNull]
        private object Execute([NotNull] string query)
        {
            var context = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/XmlContentItem");

            var q = new Query();

            return q.Execute(query, context);
        }

        private object Evaluate([NotNull] string query)
        {
            var context = Project.ProjectItems.FirstOrDefault(i => i.QualifiedName == "/sitecore/content/XmlContentItem");

            var q = new Query();

            return q.Evaluate(query, context);
        }

        [Test]
        public void QueryTests()
        {
            Assert.AreEqual(2, Evaluate("1 + 1"));
            Assert.AreEqual("XmlContentItem", Execute("@@name"));
            Assert.AreEqual(true, Evaluate("@@name = 'XmlContentItem'"));
        }
    }
}
