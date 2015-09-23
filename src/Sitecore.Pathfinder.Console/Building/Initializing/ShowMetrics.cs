using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Building.Initializing
{
    [Export(typeof(ITask))]
    public class ShowMetrics : TaskBase
    {
        public ShowMetrics() : base("show-metrics")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.Writeline("Project Metrics");
            context.Trace.Writeline("---------------");
            context.Trace.Writeline("Items", context.Project.Items.OfType<Item>().Count().ToString());
            context.Trace.Writeline("Templates", context.Project.Items.OfType<Template>().Count().ToString());
            context.Trace.Writeline("Files", context.Project.Items.OfType<Projects.Files.File>().Count().ToString());
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Shows various information about the project.");
        }
    }
}