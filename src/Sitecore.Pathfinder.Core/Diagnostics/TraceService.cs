// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
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

            var ignoredMessages = new List<int>();
            foreach (var pair in configuration.GetSubKeys("messages"))
            {
                ignoredMessages.AddRange(configuration.GetCommaSeparatedStringList("messages:" + pair.Key + ":disabled").Select(int.Parse));
            }

            IgnoredMessages = ignoredMessages;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IConsoleService Console { get; }

        [NotNull]
        protected IEnumerable<int> IgnoredMessages { get; }

        public void TraceError(string text, string details = "")
        {
            TraceError(0, text, details);
        }

        public void TraceError(int msg, string text, string details = "")
        {
            Write(msg, text, Severity.Error, string.Empty, TextSpan.Empty, details);
        }

        public void TraceError(string text, string fileName, TextSpan span, string details = "")
        {
            TraceError(0, text, fileName, span, details);
        }

        public void TraceError(int msg, string text, string fileName, TextSpan span, string details = "")
        {
            Write(msg, text, Severity.Error, fileName, span, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceError(string text, ISourceFile sourceFile, string details = "")
        {
            TraceError(0, text, sourceFile, details);
        }

        public void TraceError(int msg, string text, ISourceFile sourceFile, string details = "")
        {
            Write(msg, text, Severity.Error, sourceFile.AbsoluteFileName, TextSpan.Empty, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceError(string text, ITextNode textNode, string details = "")
        {
            TraceError(0, text, textNode, details);
        }

        public void TraceError(int msg, string text, ITextNode textNode, string details = "")
        {
            Write(msg, text, Severity.Error, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceInformation(string text, string details = "")
        {
            TraceInformation(0, text, details);
        }

        public void TraceInformation(int msg, string text, string details = "")
        {
            Write(msg, text, Severity.Information, string.Empty, TextSpan.Empty, details);

            if (Configuration.GetBool(Constants.Configuration.Debug))
            {
                Debugger.Launch();
            }
        }

        public void TraceInformation(string text, string fileName, TextSpan span, string details = "")
        {
            TraceInformation(0, text, fileName, span, details);
        }

        public void TraceInformation(int msg, string text, string fileName, TextSpan span, string details = "")
        {
            Write(msg, text, Severity.Information, fileName, span, details);
        }

        public void TraceInformation(string text, ITextNode textNode, string details = "")
        {
            TraceInformation(0, text, textNode, details);
        }

        public void TraceInformation(int msg, string text, ITextNode textNode, string details = "")
        {
            Write(msg, text, Severity.Information, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, details);
        }

        public void TraceInformation(int msg, string text, ISourceFile sourceFile, string details = "")
        {
            Write(msg, text, Severity.Information, sourceFile.AbsoluteFileName, TextSpan.Empty, details);
        }

        public void TraceWarning(string text, string details = "")
        {
            TraceWarning(0, text, details);
        }

        public void TraceWarning(int msg, string text, string details = "")
        {
            Write(msg, text, Severity.Warning, string.Empty, TextSpan.Empty, details);
        }

        public void TraceWarning(string text, string fileName, TextSpan span, string details = "")
        {
            TraceWarning(0, text, fileName, span, details);
        }

        public void TraceWarning(int msg, string text, string fileName, TextSpan span, string details = "")
        {
            Write(msg, text, Severity.Warning, fileName, span, details);
        }

        public void TraceWarning(string text, ITextNode textNode, string details = "")
        {
            TraceWarning(0, text, textNode, details);
        }

        public void TraceWarning(int msg, string text, ITextNode textNode, string details = "")
        {
            Write(msg, text, Severity.Warning, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, details);
        }

        public void TraceWarning(int msg, string text, ISourceFile sourceFile, string details = "")
        {
            Write(msg, text, Severity.Warning, sourceFile.AbsoluteFileName, TextSpan.Empty, details);
        }

        public void WriteLine(string text, string details = "")
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += @": " + details;
            }

            Console.WriteLine(text);
        }

        protected virtual void Write(int msg, [NotNull] string text, Severity severity, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details)
        {
            if (IgnoredMessages.Contains(msg))
            {
                return;
            }

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

            var lineInfo = textSpan.Length == 0 ? $"({textSpan.LineNumber},{textSpan.LinePosition})" : $"({textSpan.LineNumber},{textSpan.LinePosition},{textSpan.LineNumber},{textSpan.LinePosition + textSpan.Length})";

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
            Console.Write(" SCC{0}: ", msg.ToString("0000"));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }
    }
}
