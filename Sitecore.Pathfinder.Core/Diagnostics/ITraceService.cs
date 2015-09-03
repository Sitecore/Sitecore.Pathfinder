// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Diagnostics
{
    public interface ITraceService
    {
        void TraceError([Localizable(true)] [NotNull] string text, [NotNull] string details = "");

        void TraceError([Localizable(true)] [NotNull] string text, [NotNull] string fileName, TextPosition position, [NotNull] string details = "");

        void TraceError([Localizable(true)] [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "");

        void TraceError([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceInformation([Localizable(true)] [NotNull] string text, [NotNull] string details = "");

        void TraceInformation([Localizable(true)] [NotNull] string text, [NotNull] string fileName, TextPosition position, [NotNull] string details = "");

        void TraceInformation([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void TraceWarning([Localizable(true)] [NotNull] string text, [NotNull] string details = "");

        void TraceWarning([Localizable(true)] [NotNull] string text, [NotNull] string fileName, TextPosition position, [NotNull] string details = "");

        void TraceWarning([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "");

        void Writeline([Localizable(true)] [NotNull] string text, [NotNull] string details = "");
    }
}
