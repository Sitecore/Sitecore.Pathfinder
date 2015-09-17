// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building.Checking
{
    [Export(typeof(ITask))]
    public class CheckProject : TaskBase
    {
        public CheckProject() : base("check-project")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Texts.Checking___);

            TraceDiagnostics(context);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Checks the project for warnings and errors.");
        }

        protected void TraceDiagnostics([NotNull] IBuildContext context)
        {
            foreach (var diagnostic in context.Project.Diagnostics)
            {
                switch (diagnostic.Severity)
                {
                    case Severity.Error:
                        context.Trace.TraceError(diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    case Severity.Warning:
                        context.Trace.TraceWarning(diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                    default:
                        context.Trace.TraceInformation(diagnostic.Text, diagnostic.FileName, diagnostic.Span);
                        break;
                }
            }

            var errors = context.Project.Diagnostics.Count(d => d.Severity == Severity.Error);
            var warnings = context.Project.Diagnostics.Count(d => d.Severity == Severity.Warning);
            var messages = context.Project.Diagnostics.Count(d => d.Severity == Severity.Information);

            context.Trace.TraceInformation($"Errors: {errors}, warnings: {warnings}, messages: {messages}");
        }
    }
}
