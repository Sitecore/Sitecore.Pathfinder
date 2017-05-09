// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    public class IniFileConfigurationSource : ConfigurationSource
    {
        public IniFileConfigurationSource([CanBeNull] string path) : this(path, false)
        {
        }

        public IniFileConfigurationSource([CanBeNull] string path, bool optional)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Error_InvalidFilePath, nameof(path));
            }

            Optional = optional;
            Path = PathResolver.ResolveAppRelativePath(path);
        }

        public bool Optional { get; }

        [NotNull]
        public string Path { get; }

        public override void Load()
        {
            if (!File.Exists(Path))
            {
                if (!Optional)
                {
                    throw new FileNotFoundException(string.Format(Resources.Error_FileNotFound, Path), Path);
                }

                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                using (var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    Load(fileStream);
                }
            }
        }

        internal void Load([NotNull] Stream stream)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var streamReader = new StreamReader(stream))
            {
                var s = string.Empty;
                while (streamReader.Peek() != -1)
                {
                    var str2 = streamReader.ReadLine() ?? string.Empty;
                    var str3 = str2.Trim();
                    if (!string.IsNullOrWhiteSpace(str3) && str3[0] != 59 && str3[0] != 35 && str3[0] != 47)
                    {
                        if (str3[0] == 91)
                        {
                            var str4 = str3;
                            var index = str4.Length - 1;
                            if (str4[index] == 93)
                            {
                                s = str3.Substring(1, str3.Length - 2) + ":";
                                continue;
                            }
                        }

                        var length = str3.IndexOf('=');
                        if (length < 0)
                        {
                            throw new FormatException(Resources.FormatError_UnrecognizedLineFormat(str2));
                        }

                        var key = s + str3.Substring(0, length).Trim();
                        var str5 = str3.Substring(length + 1).Trim();
                        if (str5.Length > 1 && str5[0] == 34)
                        {
                            var str4 = str5;
                            var index = str4.Length - 1;
                            if (str4[index] == 34)
                            {
                                str5 = str5.Substring(1, str5.Length - 2);
                            }
                        }

                        if (dictionary.ContainsKey(key))
                        {
                            throw new FormatException(Resources.FormatError_KeyIsDuplicated(key));
                        }

                        dictionary[key] = str5;
                    }
                }
            }

            Data = dictionary;
        }
    }
}
