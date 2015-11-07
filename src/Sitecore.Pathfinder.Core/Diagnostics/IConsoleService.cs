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
        string Pick([NotNull] string promptText, [NotNull] Dictionary<string, string> options);

        [CanBeNull]
        string ReadLine();

        [NotNull]
        string ReadLine([NotNull] string promptText, [NotNull] string defaultValue);

        void Write([NotNull] string format, [NotNull] [ItemCanBeNull] params object[] arg);

        void WriteLine([NotNull] string format, [NotNull] [ItemCanBeNull] params object[] arg);

        void WriteLine();

        [CanBeNull]
        bool? YesNo([NotNull] string promptText, [CanBeNull] bool? defaultValue);
    }
}
