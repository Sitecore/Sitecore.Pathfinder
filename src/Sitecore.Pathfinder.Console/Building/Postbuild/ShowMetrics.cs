// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Renderings;

namespace Sitecore.Pathfinder.Building.Postbuild
{
    public class ShowMetrics : BuildTaskBase
    {
        public ShowMetrics() : base("show-metrics")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            var items = context.Project.Items.Count();
            var templates = context.Project.Templates.Count();
            var renderings = context.Project.ProjectItems.OfType<Rendering>().Count();
            var media = context.Project.ProjectItems.OfType<MediaFile>().Count();
            var files = context.Project.Files.Count();

            context.Trace.WriteLine($"Project metrics: {items} items, {templates} templates, {media} media files, {renderings} renderings, {files} files");
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Shows various information about the project.");
        }
    }
}
