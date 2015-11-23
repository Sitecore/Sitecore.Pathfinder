// © 2015 Sitecore Corporation A/S. All rights reserved.

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

        public int Indentation { get; set; } = 4;

        protected int Indent { get; set; }

        [NotNull]
        protected TextWriter InnerWriter { get; }

        public void WriteAttributeString([NotNull] string key, [NotNull] string value = "")
        {
            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(" : ");
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
            InnerWriter.Write(" : ");
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
            InnerWriter.Write(" : ");
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
            InnerWriter.Write(" : ");
            InnerWriter.WriteLine(value);
        }

        public void WriteEndElement()
        {
            Indent--;
        }

        public void WriteStartElement([NotNull] string key, [NotNull] string value = "", bool includeDash = true)
        {
            if (includeDash && !key.StartsWith("-"))
            {
                key = "- " + key;
            }

            InnerWriter.Write(new string(' ', Indent * Indentation));
            InnerWriter.Write(key);
            InnerWriter.Write(" : ");
            InnerWriter.WriteLine(value);
            Indent++;
        }
    }
}
