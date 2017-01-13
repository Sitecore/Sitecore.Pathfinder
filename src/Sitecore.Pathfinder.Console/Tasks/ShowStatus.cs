// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ShowStatus : BuildTaskBase, IIgnoreAbortedTask
    {
        [ImportingConstructor]
        public ShowStatus([NotNull] IHostService host, [NotNull] IConsoleService console) : base("show-status")
        {
            Host = host;
            Console = console;
        }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IHostService Host { get; }

        public override void Run(IBuildContext context)
        {
            if (!context.IsProjectLoaded)
            {
                return;
            }

            var project = context.LoadProject();

            var items = project.Items.Count();
            var templates = project.Templates.Count();
            var renderings = project.ProjectItems.OfType<Rendering>().Count();
            var media = project.ProjectItems.OfType<MediaFile>().Count();
            var files = project.Files.Count();

            context.Trace.WriteLine($"Project metrics: {items} items, {templates} templates, {media} media files, {renderings} renderings, {files} files");

            context.ErrorCode = project.Diagnostics.Any(d => d.Severity == Severity.Warning || d.Severity == Severity.Error) ? 1 : 0;

            if (Host.Stopwatch != null)
            {
                Host.Stopwatch.Stop();
                Console.Write(Texts.Time___0_ms, Host.Stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0"));
                Console.Write(@", ");
            }

            Console.Write(Texts.Ducats___0_, project.Ducats.ToString("#,##0"));
            Console.WriteLine();
        }
    }
}
