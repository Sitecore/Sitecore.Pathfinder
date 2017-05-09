// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class CheckProject : BuildTaskBase
    {
        [ImportingConstructor]
        public CheckProject([NotNull] ICheckerService checkerService) : base("check-project")
        {
            CheckerService = checkerService;
            Alias = "check";
            Shortcut = "c";
        }

        [NotNull]
        protected ICheckerService CheckerService { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.C1041, Texts.Checking___);

            var treatWarningsAsErrors = context.Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);

            var project = context.LoadProject();

            var diagnostics = project.Diagnostics.ToArray();

            context.Trace.TraceDiagnostics(diagnostics, treatWarningsAsErrors);

            var errors = diagnostics.Count(d => d.Severity == Severity.Error);
            var warnings = diagnostics.Count(d => d.Severity == Severity.Warning);
            var messages = diagnostics.Count(d => d.Severity == Severity.Information);
            var checkersCount = CheckerService.EnabledCheckersCount;
            var references = project.ProjectItems.Sum(i => i.References.Count);

            if (treatWarningsAsErrors)
            {
                errors += warnings;
                warnings = 0;
            }

            context.Trace.TraceInformation(Msg.C1042, $"Checks: {checkersCount}, references: {references}, errors: {errors}, warnings: {warnings}, messages: {messages}");

            if (context.Configuration.GetBool(Constants.Configuration.CheckProject.StopOnErrors, true) && errors > 0)
            {
                context.IsAborted = true;
            }
        }
    }
}
