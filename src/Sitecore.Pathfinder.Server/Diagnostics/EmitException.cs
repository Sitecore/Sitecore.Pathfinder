// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel;

namespace Sitecore.Pathfinder.Diagnostics
{
    public class EmitException : Exception
    {
        public EmitException([Localizable(true), NotNull] string text) : base(text)
        {
            Text = text;

            Details = string.Empty;
        }

        public EmitException([Localizable(true), NotNull] string text, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
        {
            Text = text;
            Details = details;
        }

        [NotNull]
        public string Details { get; }

        [NotNull]
        public string Text { get; }
    }
}
