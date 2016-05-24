// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class CheckProject : BuildTaskBase
    {
        [ImportingConstructor]
        public CheckProject([NotNull] ICheckerService checkerService) : base("check-project")
        {
            CheckerService = checkerService;
        }

        [NotNull]
        protected ICheckerService CheckerService { get; }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.C1041, Texts.Checking___);

            var treatWarningsAsErrors = context.Configuration.GetBool(Constants.Configuration.CheckProject.TreatWarningsAsErrors);

            context.Trace.TraceDiagnostics(context.Project.Diagnostics, treatWarningsAsErrors);

            var errors = context.Project.Diagnostics.Count(d => d.Severity == Severity.Error);
            var warnings = context.Project.Diagnostics.Count(d => d.Severity == Severity.Warning);
            var messages = context.Project.Diagnostics.Count(d => d.Severity == Severity.Information);
            var checkers = CheckerService.EnabledCheckersCount;

            if (treatWarningsAsErrors)
            {
                errors += warnings;
                warnings = 0;
            }

            context.Trace.TraceInformation(Msg.C1042, $"Checks: {checkers}, errors: {errors}, warnings: {warnings}, messages: {messages}");

            if (context.Configuration.GetBool(Constants.Configuration.CheckProject.StopOnErrors, true) && errors > 0)
            {
                context.IsAborted = true;
            }
        }
    }
}
