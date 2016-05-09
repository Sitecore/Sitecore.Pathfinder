// © 2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking.Checkers
{
    public class Check
    {
        [NotNull]
        protected Diagnostic Error([NotNull] string text)
        {
            return new Diagnostic(0, string.Empty, TextSpan.Empty, Severity.Error, GetText(text));
        }

        [NotNull]
        protected Diagnostic Error([NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(0, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Error, GetText(text));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Error, GetText(text));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] string details, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Error, GetText(text, details));
        }

        [NotNull]
        protected Diagnostic Information([NotNull] string text)
        {
            return new Diagnostic(0, string.Empty, TextSpan.Empty, Severity.Information, GetText(text));
        }

        [NotNull]
        protected Diagnostic Information([NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(0, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Information, GetText(text));
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Information, GetText(text));
        }

        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] string details, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Information, GetText(text, details));
        }

        [NotNull]
        protected Diagnostic Warning([NotNull] string text)
        {
            return new Diagnostic(0, string.Empty, TextSpan.Empty, Severity.Warning, GetText(text));
        }

        [NotNull]
        protected Diagnostic Warning([NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);

            return new Diagnostic(0, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Warning, GetText(text));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);

            return new Diagnostic(msg, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Warning, GetText(text));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] string details, [NotNull, ItemNotNull] params IHasSourceTextNodes[] textNodes)
        {
            var textNode = TraceHelper.GetTextNode(textNodes);

            return new Diagnostic(msg, textNode.Snapshot.SourceFile.RelativeFileName, textNode.TextSpan, Severity.Warning, GetText(text, details));
        }

        [NotNull]
        private string GetText([NotNull] string text, [NotNull] string details = "")
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            text += " [" + GetType().Name + "]";

            return text;
        }
    }
}
