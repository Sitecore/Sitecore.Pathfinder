// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public class YamlTextWriter
    {
        public YamlTextWriter([NotNull] TextWriter innerWriter)
        {
            InnerWriter = innerWriter;
        }

        public YamlTextWriter([NotNull] TextWriter innerWriter, int indent)
        {
            InnerWriter = innerWriter;
            Indent = indent;
        }

        public int Indent { get; protected set; }

        public int Indentation { get; set; } = 4;

        [NotNull]
        protected TextWriter InnerWriter { get; }

        public void Write([NotNull] string text)
        {
            InnerWriter.Write(text);
        }

        public void WriteAttributeString([NotNull] string key, [NotNull] string value = "")
        {
            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(": ");
            InnerWriter.WriteLine(value);
        }

        public void WriteAttributeStringIf([NotNull] string key, [NotNull] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(": ");
            InnerWriter.WriteLine(value);
        }

        public void WriteAttributeStringIf([NotNull] string key, int value, int defaultValue = 0)
        {
            if (value == defaultValue)
            {
                return;
            }

            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(": ");
            InnerWriter.WriteLine(value);
        }

        public void WriteAttributeStringIf([NotNull] string key, bool value, bool defaultValue = true)
        {
            if (value == defaultValue)
            {
                return;
            }

            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(": ");
            InnerWriter.WriteLine(value);
        }

        public void WriteEndElement()
        {
            Indent--;
        }

        public void WriteLine([NotNull] string text)
        {
            InnerWriter.WriteLine(text);
        }

        public void WriteStartElement([NotNull] string key, [NotNull] string value = "")
        {
            if (!key.StartsWith("-"))
            {
                key = "- " + key;
            }

            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(": ");
            InnerWriter.WriteLine(value);
            Indent++;
        }
    }
}
