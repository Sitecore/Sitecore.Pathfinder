// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel;

namespace Sitecore.Pathfinder.Diagnostics
{
    public class RetryableEmitException : EmitException
    {
        public RetryableEmitException([Localizable(true), NotNull] string text, [NotNull] string details = "") : base(text, details)
        {
        }
    }
}
