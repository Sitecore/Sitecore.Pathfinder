// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Diagnostics
{
    public delegate void TraceOutDelegate(int msg, [NotNull] string text, Severity severity, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details);

    public interface ITraceService
    {
        [CanBeNull]
        TraceOutDelegate Out { get; }

        void SetOut([CanBeNull] TraceOutDelegate traceOut);

        void TraceError([Localizable(true), NotNull] string text, [NotNull] string details = "");

        void TraceError([Localizable(true), NotNull] string text, [NotNull] string fileName, TextSpan span, [NotNull] string details = "");

        void TraceError([Localizable(true), NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "");

        void TraceError([Localizable(true), NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceError(int msg, [Localizable(true), NotNull] string text, [NotNull] string details = "");

        void TraceError(int msg, [Localizable(true), NotNull] string text, [NotNull] string fileName, TextSpan span, [NotNull] string details = "");

        void TraceError(int msg, [Localizable(true), NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "");

        void TraceError(int msg, [Localizable(true), NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceInformation([Localizable(true), NotNull] string text, [NotNull] string details = "");

        void TraceInformation([Localizable(true), NotNull] string text, [NotNull] string fileName, TextSpan span, [NotNull] string details = "");

        void TraceInformation([Localizable(true), NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceInformation(int msg, [Localizable(true), NotNull] string text, [NotNull] string details = "");

        void TraceInformation(int msg, [Localizable(true), NotNull] string text, [NotNull] string fileName, TextSpan span, [NotNull] string details = "");

        void TraceInformation(int msg, [Localizable(true), NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceInformation(int msg, [Localizable(true), NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "");

        void TraceWarning([Localizable(true), NotNull] string text, [NotNull] string details = "");

        void TraceWarning([Localizable(true), NotNull] string text, [NotNull] string fileName, TextSpan span, [NotNull] string details = "");

        void TraceWarning([Localizable(true), NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceWarning(int msg, [Localizable(true), NotNull] string text, [NotNull] string details = "");

        void TraceWarning(int msg, [Localizable(true), NotNull] string text, [NotNull] string fileName, TextSpan span, [NotNull] string details = "");

        void TraceWarning(int msg, [Localizable(true), NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceWarning(int msg, [Localizable(true), NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "");

        void WriteLine([Localizable(true), NotNull] string text, [NotNull] string details = "");
    }
}
