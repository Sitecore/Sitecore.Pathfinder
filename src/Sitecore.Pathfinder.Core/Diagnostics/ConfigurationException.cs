// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel;
using Sitecore.Patfhfinder.Diagnostics;

namespace Sitecore.Pathfinder.Diagnostics
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException([NotNull, Localizable(true)]  string text, [NotNull] string details = "") : base(text + (string.IsNullOrEmpty(details) ? ": " + details : string.Empty))
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
