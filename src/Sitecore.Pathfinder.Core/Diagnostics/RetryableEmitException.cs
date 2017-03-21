// © 2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Diagnostics
{
    public class RetryableEmitException : EmitException
    {
        public RetryableEmitException(int msg, [Localizable(true), NotNull]  string text, [NotNull] ISnapshot snapshot, [NotNull] string details = "") : base(msg, text, snapshot, details)
        {
        }

        public RetryableEmitException(int msg, [Localizable(true), NotNull]  string text, [NotNull] ITextNode textNode, [NotNull] string details = "") : base(msg, text, textNode, details)
        {
        }
    }
}
