// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;

namespace Sitecore.Pathfinder.Diagnostics
{
    public interface IConsoleService
    {
        ConsoleColor BackgroundColor { get; set; }

        ConsoleColor ForegroundColor { get; set; }

        bool IsInteractive { get; set; }

        [NotNull]
        string Pick([NotNull, Localizable(true)] string promptText, [NotNull] Dictionary<string, string> options, [NotNull] string configName = "");

        [CanBeNull]
        string ReadLine([NotNull] string configName = "");

        [NotNull]
        string ReadLine([NotNull] string promptText, [NotNull] string defaultValue, [NotNull] string configName = "");

        void Write([NotNull, Localizable(true)] string format, [NotNull, ItemCanBeNull] params object[] arg);

        void Write([NotNull, Localizable(true)] string text);

        void WriteLine([NotNull, Localizable(true)] string format, [NotNull, ItemCanBeNull] params object[] arg);

        void WriteLine([NotNull, Localizable(true)] string text);

        void WriteLine();

        [CanBeNull]
        bool? YesNo([NotNull, Localizable(true)] string promptText, [CanBeNull] bool? defaultValue, [NotNull] string configName = "");
    }
}
