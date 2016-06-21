// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ShowStatus : BuildTaskBase, IAlwaysRunTask
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

            var items = context.Project.Items.Count();
            var templates = context.Project.Templates.Count();
            var renderings = context.Project.ProjectItems.OfType<Rendering>().Count();
            var media = context.Project.ProjectItems.OfType<MediaFile>().Count();
            var files = context.Project.Files.Count();

            context.Trace.WriteLine($"Project metrics: {items} items, {templates} templates, {media} media files, {renderings} renderings, {files} files");

            context.ErrorCode = context.Project.Diagnostics.Any(d => d.Severity == Severity.Warning || d.Severity == Severity.Error) ? 1 : 0;

            if (Host.Stopwatch != null)
            {
                Host.Stopwatch.Stop();
                Console.Write(Texts.Time___0_ms, Host.Stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0"));
                Console.Write(@", ");
            }

            Console.Write(Texts.Ducats___0_, context.Project.Ducats.ToString("#,##0"));
            Console.WriteLine();
        }
    }
}
