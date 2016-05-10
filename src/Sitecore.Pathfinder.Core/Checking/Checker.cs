// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking
{
    public class Checker
    {
        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, fileName, textSpan, Severity.Error, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, sourceFile.AbsoluteFileName, TextSpan.Empty, Severity.Error, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, Severity.Error, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, fileName, textSpan, Severity.Information, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, sourceFile.AbsoluteFileName, TextSpan.Empty, Severity.Information, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, Severity.Information, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, string.Empty, TextSpan.Empty, Severity.Warning, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, fileName, textSpan, Severity.Warning, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, sourceFile.AbsoluteFileName, TextSpan.Empty, Severity.Warning, GetText(text, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "", [NotNull, System.Runtime.CompilerServices.CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, Severity.Warning, GetText(text, checkerName));
        }

        [NotNull]
        private string GetText([NotNull] string text, [NotNull] string checkerName, [NotNull] string details = "")
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            text += " [" + checkerName + "]";

            return text;
        }
    }
}
