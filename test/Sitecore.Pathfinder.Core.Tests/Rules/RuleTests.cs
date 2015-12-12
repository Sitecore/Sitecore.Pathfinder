// © 2015 Sitecore Corporation A/S. All rights reserved.

using NUnit.Framework;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Rules.Contexts;

namespace Sitecore.Pathfinder.Rules
{
    [TestFixture]
    public class RuleTests : Tests
    {
        [NotNull]
        public IProject Project { get; set; }

        [TestFixtureSetUp]
        public void Startup()
        {
            Start(GoodWebsite);
            Project = Services.ProjectService.LoadProjectFromConfiguration();
        }

        [Test]
        public void NoContentItemsRule()
        {
            var rule = Services.RuleService.ParseRule("no-content-rule");

            foreach (var item in Project.Items)
            {
                var context = new RuleContext(item);
                rule.Execute(context);
            }
        }

        [Test]
        public void SimpleRule()
        {
            var rule = Services.RuleService.ParseRule("simple-rule");

            var context = new RuleContext(Project.Items);

            Assert.IsTrue(rule.EvaluateIf(context));
            rule.Execute(context);
        }
    }
}
