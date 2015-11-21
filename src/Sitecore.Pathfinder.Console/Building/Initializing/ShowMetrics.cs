using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Building.Initializing
{
    public class ShowMetrics : BuildTaskBase
    {
        public ShowMetrics() : base("show-metrics")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.WriteLine("Project Metrics");
            context.Trace.WriteLine("---------------");
            context.Trace.WriteLine("Items", context.Project.ProjectItems.OfType<Item>().Count().ToString());
            context.Trace.WriteLine("Templates", context.Project.ProjectItems.OfType<Template>().Count().ToString());
            context.Trace.WriteLine("Files", context.Project.ProjectItems.OfType<Projects.Files.File>().Count().ToString());
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Shows various information about the project.");
        }
    }
}