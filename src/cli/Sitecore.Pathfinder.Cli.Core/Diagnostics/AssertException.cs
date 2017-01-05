using System;

namespace Sitecore.Pathfinder.Diagnostics
{
    public class AssertException : Exception
    {
        public AssertException([NotNull]  string text) : base(text)
        {
            Text = text;

            Details = string.Empty;
            FileName = string.Empty;
            Details = string.Empty;
        }

        [NotNull]
        public string Details { get; }

        [NotNull]
        public string FileName { get; }

        [NotNull]
        public string Text { get; }
    }
}