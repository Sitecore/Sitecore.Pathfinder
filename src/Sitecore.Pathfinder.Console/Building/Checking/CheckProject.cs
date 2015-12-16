// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.Checking
{
    public class CheckProject : BuildTaskBase
    {
        public CheckProject() : base("check-project")
        {
            CanRunWithoutConfig = true;
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.C1041, Texts.Checking___);

            TraceDiagnostics(context);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Checks the project for warnings and errors.");
            helpWriter.Remarks.Write("SETTINGS:");
            helpWriter.Remarks.Write("  check-project:disabled-categories - Disables checker categories (Items, Fields, Templates, TemplateFields, Media).");
            helpWriter.Remarks.Write("  check-project:disabled-checkers - Disables specific checkers.");
        }

        protected virtual void TraceDiagnostics([NotNull] IBuildContext context)
        {
            foreach (var diagnostic in context.Project.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case Severity.Error:
                        context.Trace.TraceError(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    case Severity.Warning:
                        context.Trace.TraceWarning(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                     case Severity.Information:
                        context.Trace.TraceInformation(diagnostic.Msg, diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var errors = context.Project.Diagnostics.Count(d => d.Severity == Severity.Error);
            var warnings = context.Project.Diagnostics.Count(d => d.Severity == Severity.Warning);
            var messages = context.Project.Diagnostics.Count(d => d.Severity == Severity.Information);

            context.Trace.TraceInformation(Msg.C1042, $"Errors: {errors}, warnings: {warnings}, messages: {messages}");
        }
    }
}
