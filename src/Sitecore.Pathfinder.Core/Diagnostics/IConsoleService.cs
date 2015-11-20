// © 2015 Sitecore Corporation A/S. All rights reserved.

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
        string Pick([NotNull] string promptText, [NotNull] Dictionary<string, string> options, [NotNull] string configName = "");

        [CanBeNull]
        string ReadLine([NotNull] string configName = "");

        [NotNull]
        string ReadLine([NotNull] string promptText, [NotNull] string defaultValue, [NotNull] string configName = "");

        void Write([NotNull] string format, [NotNull] [ItemCanBeNull] params object[] arg);

        void Write([NotNull] string text);

        void WriteLine([NotNull] string format, [NotNull] [ItemCanBeNull] params object[] arg);

        void WriteLine([NotNull] string text);

        void WriteLine();

        [CanBeNull]
        bool? YesNo([NotNull] string promptText, [CanBeNull] bool? defaultValue, [NotNull] string configName = "");
    }
}
