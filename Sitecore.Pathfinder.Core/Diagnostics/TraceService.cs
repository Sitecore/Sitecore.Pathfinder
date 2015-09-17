// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Diagnostics
{
    public enum Severity
    {
        Information,

        Warning,

        Error
    }

    [Export(typeof(ITraceService))]
    public class TraceService : ITraceService
    {
        [ImportingConstructor]
        public TraceService([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public void TraceError(string text, string details = "")
        {
            Write(text, Severity.Error, string.Empty, TextSpan.Empty, details);
        }

        public void TraceError(string text, string fileName, TextSpan span, string details = "")
        {
            Write(text, Severity.Error, fileName, span, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceError(string text, ISourceFile sourceFile, string details = "")
        {
            Write(text, Severity.Error, sourceFile.FileName, TextSpan.Empty, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceError(string text, ITextNode textNode, string details = "")
        {
            Write(text, Severity.Error, textNode.Snapshot.SourceFile.FileName, textNode.Span, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceInformation(string text, string details = "")
        {
            Write(text, Severity.Information, string.Empty, TextSpan.Empty, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceInformation(string text, string fileName, TextSpan span, string details = "")
        {
            Write(text, Severity.Information, fileName, span, details);
        }

        public void TraceInformation(string text, ITextNode textNode, string details = "")
        {
            Write(text, Severity.Information, textNode.Snapshot.SourceFile.FileName, textNode.Span, details);
        }

        public void TraceWarning(string text, string details = "")
        {
            Write(text, Severity.Warning, string.Empty, TextSpan.Empty, details);
        }

        public void TraceWarning(string text, string fileName, TextSpan span, string details = "")
        {
            Write(text, Severity.Warning, fileName, span, details);
        }

        public void TraceWarning(string text, ITextNode textNode, string details = "")
        {
            Write(text, Severity.Warning, textNode.Snapshot.SourceFile.FileName, textNode.Span, details);
        }

        public void Writeline(string text, string details = "")
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += @": " + details;
            }

            Console.WriteLine(text);
        }

        protected virtual void Write([NotNull] string text, Severity severity, [NotNull] string fileName, TextSpan span, [NotNull] string details)
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            var fileInfo = !string.IsNullOrEmpty(fileName) ? fileName : "scc.cmd";

            var solutionDirectory = Configuration.Get(Constants.Configuration.SolutionDirectory);
            if (!string.IsNullOrEmpty(solutionDirectory))
            {
                if (fileInfo.StartsWith(solutionDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    fileInfo = fileInfo.Mid(solutionDirectory.Length + 1);
                }
            }

            var lineInfo = span.LineLength == 0 ? $"({span.LineNumber},{span.LinePosition})" : $"({span.LineNumber},{span.LinePosition},{span.LineNumber},{span.LinePosition + span.LineLength})";

            Console.WriteLine($"{fileInfo}{lineInfo}: {severity.ToString().ToLowerInvariant()} SCC0000: {text}");
        }
    }
}
