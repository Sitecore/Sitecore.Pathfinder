// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Building
{
    public class ConsoleService
    {
        public ConsoleColor BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }

        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        public bool IsInteractive { get; set; } = true;

        [NotNull]
        public string Pick([NotNull] string promptText, [NotNull] Dictionary<string, string> options)
        {
            Console.WriteLine();

            var index = 1;
            foreach (var option in options)
            {
                Console.WriteLine($"{index}: {option.Key}");
                index++;
            }

            while (true)
            {
                Console.Write(promptText);
                var input = Console.ReadLine();

                if (input == "0")
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(input))
                {
                    return options.Values.First();
                }

                int selection;
                if (!int.TryParse(input, out selection))
                {
                    continue;
                }

                if (selection < 1)
                {
                    continue;
                }

                if (selection > options.Count)
                {
                    continue;
                }

                return options.Values.ElementAt(selection - 1);
            }
        }

        [NotNull]
        public string ReadLine([NotNull] string promptText, [NotNull] string defaultValue)
        {
            if (!IsInteractive)
            {
                return defaultValue;
            }

            Console.Write(promptText);

            var input = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                input = defaultValue;
            }

            return input;
        }

        public void Write([NotNull] string format, [NotNull] params object[] arg)
        {
            Console.Write(format, arg);
        }

        public void WriteLine([NotNull] string format, [NotNull] params object[] arg)
        {
            Console.WriteLine(format, arg);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        [CanBeNull]
        public bool? YesNo([NotNull] string promptText, [CanBeNull] bool? defaultValue)
        {
            if (!IsInteractive)
            {
                return defaultValue;
            }

            while (true)
            {
                Console.Write(promptText);

                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return defaultValue;
                }

                if (string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (string.Equals(input, "N", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                WriteLine("Enter 'Y' or 'N'.");
            }
        }
    }
}
