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
        public TraceService([NotNull] IConfiguration configuration, [NotNull] IConsoleService console)
        {
            Configuration = configuration;
            Console = console;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

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
            Write(text, Severity.Error, sourceFile.AbsoluteFileName, TextSpan.Empty, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceError(string text, ITextNode textNode, string details = "")
        {
            Write(text, Severity.Error, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, details);

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
            Write(text, Severity.Information, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, details);
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
            Write(text, Severity.Warning, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, details);
        }

        public void WriteLine(string text, string details = "")
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

            var projectDirectory = Configuration.Get(Constants.Configuration.ProjectDirectory);
            if (!string.IsNullOrEmpty(projectDirectory))
            {
                if (fileInfo.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    fileInfo = fileInfo.Mid(projectDirectory.Length + 1);
                }
            }

            var lineInfo = span.Length == 0 ? $"({span.LineNumber},{span.LinePosition})" : $"({span.LineNumber},{span.LinePosition},{span.LineNumber},{span.LinePosition + span.Length})";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{fileInfo}{lineInfo}: ");

            switch (severity)
            {
                case Severity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Severity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.Write(severity.ToString().ToLowerInvariant());

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" SCC0000: ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }
    }
}
