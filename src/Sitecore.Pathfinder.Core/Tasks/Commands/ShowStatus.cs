// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks.Commands
{
    [Export(typeof(ITask)), Shared]
    public class ShowStatus : BuildTaskBase
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

            var diagnostics = project.Diagnostics.ToArray();

            var errors = diagnostics.Count(d => d.Severity == Severity.Error);
            var warnings = diagnostics.Count(d => d.Severity == Severity.Warning);
            var messages = diagnostics.Count(d => d.Severity == Severity.Information);

            var treatWarningsAsErrors = context.Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);
            if (treatWarningsAsErrors)
            {
                errors += warnings;
                warnings = 0;
            }

            context.Trace.WriteLine($"Project metrics: {items} items, {templates} templates, {media} media files, {renderings} renderings, {files} files, {errors} errors, {warnings} warnings, {messages} messages");

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
