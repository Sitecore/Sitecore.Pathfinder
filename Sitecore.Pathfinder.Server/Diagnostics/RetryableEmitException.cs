// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel;
using Sitecore.Pathfinder.Documents;

namespace Sitecore.Pathfinder.Diagnostics
{
    public class RetryableEmitException : EmitException
    {
        public RetryableEmitException([Localizable(true)] [NotNull] string text, [NotNull] ISnapshot snapshot, [NotNull] string details = "") : base(text, snapshot, details)
        {
        }

        public RetryableEmitException([Localizable(true)] [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "") : base(text, textNode, details)
        {
        }
    }
}
