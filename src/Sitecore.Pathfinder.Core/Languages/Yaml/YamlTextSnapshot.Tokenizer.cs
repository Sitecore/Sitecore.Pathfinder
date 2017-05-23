// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Yaml
{
    public partial class YamlTextSnapshot
    {
        protected class Tokenizer
        {
            [ItemNotNull, NotNull]
            private readonly string[] _lines;

            private int _lineNumber;

            public Tokenizer([NotNull] string contents)
            {
                var lines = new List<string>();
                var reader = new StringReader(contents);
                var line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }

                _lines = lines.ToArray();
                _lineNumber = 0;

                Token = NextToken();
            }

            [CanBeNull]
            public Token Token { get; private set; }

            [CanBeNull]
            public Token Match()
            {
                Token = NextToken();
                return Token;
            }

            [CanBeNull]
            protected Token NextToken()
            {
                while (_lineNumber < _lines.Length && string.IsNullOrWhiteSpace(_lines[_lineNumber]))
                {
                    _lineNumber++;
                }

                if (_lineNumber >= _lines.Length)
                {
                    return null;
                }

                Token token;
                var line = _lines[_lineNumber];
                var indent = line.IndexOfNotWhitespace();
                var tabbedIndent = line.Left(indent).Replace("\t", "    ").Length;
                var isNested = line[indent] == '-';

                var keyStartIndex = isNested ? line.IndexOfNotWhitespace(indent + 1) : indent;

                var n = line.IndexOf(':');
                if (n < 0)
                {
                    var key = line.Mid(keyStartIndex).Trim();
                    var keyTextSpan = new TextSpan(_lineNumber + 1, keyStartIndex + 1, key.Length);
                    token = new Token(key, keyTextSpan, indent, isNested);
                }
                else
                {
                    var key = line.Mid(keyStartIndex, n - keyStartIndex).Trim();
                    var keyTextSpan = new TextSpan(_lineNumber + 1, keyStartIndex + 1, key.Length);
                    var value = line.Mid(n + 1).Trim();
                    TextSpan valueTextSpan;

                    if (value == ">")
                    {
                        ParseValue(out value, out valueTextSpan, " ");
                    }
                    else if (value == "|")
                    {
                        ParseValue(out value, out valueTextSpan, "\r\n");
                    }
                    else
                    {
                        valueTextSpan = new TextSpan(_lineNumber + 1, n + 1, line.Length - n);
                    }

                    token = new Token(key, keyTextSpan, value, valueTextSpan, tabbedIndent, isNested);
                }

                _lineNumber++;

                return token;
            }

            private void ParseValue([NotNull] out string value, out TextSpan valueTextSpan, [NotNull] string delimiter)
            {
                var startIndent = _lines[_lineNumber].IndexOfNotWhitespace();

                _lineNumber++;
                if (_lineNumber >= _lines.Length)
                {
                    value = string.Empty;
                    valueTextSpan = new TextSpan(0, 0, 0);
                    return;
                }

                var startLineNumber = _lineNumber;
                var length = 0;
                var sb = new StringBuilder();

                do
                {
                    var line = _lines[_lineNumber];
                    length += line.Length + Environment.NewLine.Length;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        sb.Append(delimiter);
                        _lineNumber++;
                        continue;
                    }

                    var indent = line.IndexOfNotWhitespace();
                    if (indent <= startIndent)
                    {
                        _lineNumber--;
                        break;
                    }

                    sb.Append(line.Trim());
                    sb.Append(delimiter);

                    _lineNumber++;
                }
                while (_lineNumber < _lines.Length);

                value = sb.ToString().Trim();
                valueTextSpan = new TextSpan(startLineNumber + 1, 0, length);
            }
        }
    }
}
