// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Diagnostics
{
    [Export(typeof(IConsoleService))]
    public class ConsoleService : IConsoleService
    {
        [ImportingConstructor]
        public ConsoleService([NotNull] IConfiguration configuration)
        {
            Configuration = configuration;

            IsInteractive = configuration.GetBool("interactive", true);
            IsSilent = configuration.GetBool("silent");
        }

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

        public bool IsInteractive { get; set; }

        public bool IsSilent { get; set; }

        [NotNull]
        protected IConfiguration Configuration { get; }

        public string Pick(string promptText, Dictionary<string, string> options, string configName = "")
        {
            if (!string.IsNullOrEmpty(configName))
            {
                var configValue = Configuration.GetString(configName);
                if (!string.IsNullOrEmpty(configValue))
                {
                    string value;
                    if (options.TryGetValue(configValue, out value))
                    {
                        return value;
                    }

                    if (configValue == "0")
                    {
                        return string.Empty;
                    }

                    int selection;
                    if (int.TryParse(configValue, out selection))
                    {
                        if (selection > 0 && selection <= options.Count)
                        {
                            return options.Values.ElementAt(selection - 1);
                        }
                    }

                    throw new ConfigurationException(Texts.Input_is_not_valid__ + configValue);
                }
            }

            WriteLine();

            var index = 1;
            foreach (var option in options)
            {
                WriteLine($"{index}: {option.Key}");
                index++;
            }

            while (true)
            {
                Write(promptText);
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

        public string ReadLine(string configName = "")
        {
            if (!string.IsNullOrEmpty(configName))
            {
                var configValue = Configuration.GetString(configName);
                if (!string.IsNullOrEmpty(configValue))
                {
                    return configValue;
                }
            }

            return Console.ReadLine();
        }

        public string ReadLine(string promptText, string defaultValue, string configName = "")
        {
            if (!string.IsNullOrEmpty(configName))
            {
                var configValue = Configuration.GetString(configName);
                if (!string.IsNullOrEmpty(configValue))
                {
                    return configValue;
                }
            }

            if (!IsInteractive)
            {
                return defaultValue;
            }

            Write(promptText);

            var input = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                input = defaultValue;
            }

            return input;
        }

        public void Write(string format, params object[] arg)
        {
            if (!IsSilent)
            {
                Console.Write(format, arg);
            }
        }

        public void Write(string text)
        {
            if (!IsSilent)
            {
                Console.Write(text);
            }
        }

        public void WriteLine(string format, params object[] arg)
        {
            if (!IsSilent)
            {
                Console.WriteLine(format, arg);
            }
        }

        public void WriteLine(string text)
        {
            if (!IsSilent)
            {
                Console.WriteLine(text);
            }
        }

        public void WriteLine()
        {
            if (!IsSilent)
            {
                Console.WriteLine();
            }
        }

        public bool? YesNo(string promptText, bool? defaultValue, string configName = "")
        {
            if (!string.IsNullOrEmpty(configName))
            {
                var configValue = Configuration.GetString(configName);
                if (!string.IsNullOrEmpty(configValue))
                {
                    if (string.Equals(configValue, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    if (string.Equals(configValue, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    throw new ConfigurationException(Texts.Input_is_not_valid__ + configValue);
                }
            }

            if (!IsInteractive)
            {
                return defaultValue;
            }

            while (true)
            {
                Write(promptText);

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
