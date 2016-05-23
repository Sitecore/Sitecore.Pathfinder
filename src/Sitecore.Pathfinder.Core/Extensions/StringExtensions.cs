// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class StringExtensions
    {
        private const int IsoDateLength = 8;

        private const int IsoDateTimeLength = 15;

        private const int IsoDateTimeUtcLength = 16;

        private const string IsoDateTimeUtcMarker = "Z";

        private const int IsoDateUtcLength = 9;

        [NotNull]
        public static string Append([NotNull] this string str, [NotNull] string key, [NotNull] string value, char separator = '|', char equals = '=', char escapeCharacter = '\\')
        {
            if (!string.IsNullOrEmpty(str))
            {
                str += separator;
            }

            return str + key.Escape(separator, escapeCharacter) + equals + value.Escape(separator, escapeCharacter);
        }

        [NotNull]
        public static string Capitalize([CanBeNull] this string text)
        {
            if (text == null)
            {
                return string.Empty;
            }

            return text.Left(1).ToUpperInvariant() + text.Mid(1);
        }

        [NotNull]
        public static string Clean([CanBeNull] this string s)
        {
            if (s == null || s.Length <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                sb.Append(char.IsControl(c) ? ' ' : c);
            }

            return sb.ToString();
        }

        [NotNull]
        public static string Clip([NotNull] this string text, int length, bool ellipsis)
        {
            if (text.Length <= length)
            {
                return text;
            }

            if (ellipsis)
            {
                length -= 3;
            }

            var n = text.LastIndexOf(" ", length, StringComparison.Ordinal);
            if (n < 0)
            {
                n = length;
            }

            text = text.Substring(0, n);

            if (ellipsis)
            {
                text += "...";
            }

            return text;
        }

        [NotNull]
        public static string Escape([NotNull] this string str, char character, char escapeCharacter = '\\')
        {
            var c = character.ToString(CultureInfo.CurrentCulture);
            var es = escapeCharacter.ToString(CultureInfo.CurrentCulture);

            return str.Replace(es, es + es).Replace(c, es + character);
        }

        [NotNull]
        public static string EscapeXmlElementName([NotNull] this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var chars = text.ToCharArray();

            for (var n = 1; n < chars.Length; n++)
            {
                var p = chars[n - 1];

                if (char.IsWhiteSpace(p))
                {
                    chars[n] = char.ToUpper(chars[n]);
                }
            }

            text = new string(chars);

            var result = Regex.Replace(text, "[^\\w_-]", ".");

            if (!char.IsLetter(result[0]) && result[0] != '_')
            {
                result = @"_-" + result;
            }

            return result;
        }

        public static DateTime FromIsoToDateTime([CanBeNull] this string text)
        {
            return FromIsoToDateTime(text, DateTime.MinValue);
        }

        public static DateTime FromIsoToDateTime([CanBeNull] this string text, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            try
            {
                bool isUtc;
                if (text.Length > IsoDateTimeLength && text[IsoDateTimeLength] == ':')
                {
                    var ticks = text.Substring(IsoDateTimeLength + 1);

                    if (ticks.Length > 0)
                    {
                        isUtc = ticks.EndsWith(IsoDateTimeUtcMarker, StringComparison.InvariantCultureIgnoreCase);
                        if (isUtc)
                        {
                            ticks = ticks.Replace(IsoDateTimeUtcMarker, string.Empty);
                            return new DateTime(GetLong(ticks, 0), DateTimeKind.Utc);
                        }

                        return new DateTime(GetLong(ticks, 0));
                    }
                }

                var parts = GetIsoDateParts(text, out isUtc);

                if (parts == null)
                {
                    return defaultValue;
                }

                if (parts.Length >= 6)
                {
                    return new DateTime(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5], isUtc ? DateTimeKind.Utc : DateTimeKind.Unspecified);
                }

                if (parts.Length >= 3)
                {
                    return new DateTime(parts[0], parts[1], parts[2], 0, 0, 0, isUtc ? DateTimeKind.Utc : DateTimeKind.Unspecified);
                }
            }
            catch
            {
                return defaultValue;
            }

            return defaultValue;
        }

        [NotNull]
        public static string GetSafeCodeIdentifier([NotNull] this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var chars = text.ToCharArray();

            for (var n = 1; n < chars.Length; n++)
            {
                var p = chars[n - 1];

                if (char.IsWhiteSpace(p) || p == '_')
                {
                    chars[n] = char.ToUpper(chars[n]);
                }
            }

            text = new string(chars);

            var result = Regex.Replace(text, @"\W", string.Empty).Replace(@" ", string.Empty);
            if (!char.IsLetter(result[0]) && result[0] != '_')
            {
                result = @"_" + result;
            }

            return result;
        }

        public static int IndexOfNotWhitespace([NotNull] this string text, int startIndex = 0)
        {
            for (var index = startIndex; index < text.Length; index++)
            {
                if (!char.IsWhiteSpace(text[index]))
                {
                    return index;
                }
            }

            return 0;
        }

        [NotNull]
        public static string Left([NotNull] this string text, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            return text.Length <= length ? text : text.Substring(0, length);
        }

        [NotNull]
        public static string Mid([NotNull] this string text, int start)
        {
            if (start >= text.Length || start < 0)
            {
                return string.Empty;
            }

            return text.Substring(start);
        }

        [NotNull]
        public static string Mid([NotNull] this string text, int start, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            if (start >= text.Length || start < 0)
            {
                return string.Empty;
            }

            var max = text.Length - start;

            if (length >= max)
            {
                return text.Substring(start);
            }

            return text.Substring(start, length);
        }

        [NotNull]
        public static string RemoveControlChars([NotNull] this string text)
        {
            text = text.Replace("\r", " ");
            text = text.Replace("\n", " ");

            return text;
        }

        [NotNull, ItemNotNull]
        public static string[] Split([NotNull] this string text, char separator, StringSplitOptions options)
        {
            var s = new[]
            {
                separator
            };

            return text.Split(s, options);
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<string> SplitEscaped([CanBeNull] this string str, char separator, StringSplitOptions options = StringSplitOptions.None, char escapeCharacter = '\\')
        {
            if (str == null)
            {
                yield break;
            }

            var start = 0;
            var chars = str.ToCharArray();

            for (var n = 1; n < str.Length; n++)
            {
                if (chars[n] != separator || chars[n - 1] == escapeCharacter)
                {
                    continue;
                }

                var s = str.Mid(start, n - start).Unescape(separator);
                start = n + 1;

                if (options != StringSplitOptions.RemoveEmptyEntries || !string.IsNullOrEmpty(s))
                {
                    yield return s;
                }
            }

            yield return str.Mid(start, str.Length - start).Unescape(separator);
        }

        [NotNull]
        public static string ToPascalCase([NotNull] this string text)
        {
            text = Regex.Replace(text, @"\W", @" ");

            var parts = text.Split(' ');

            var result = new StringBuilder();
            foreach (var s in parts)
            {
                result.Append(s.Capitalize());
            }

            return result.ToString();
        }

        [NotNull]
        public static string Unescape([NotNull] this string str, char character, char escapeCharacter = '\\')
        {
            var c = character.ToString(CultureInfo.CurrentCulture);
            var es = escapeCharacter.ToString(CultureInfo.CurrentCulture);

            return str.Replace(es + character, c).Replace(es + es, es);
        }

        [NotNull]
        public static string UnescapeXmlElementName([NotNull] this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (text.StartsWith("_-"))
            {
                text = text.Mid(2);
            }

            return text.Replace(".", " ").Replace(@"_-", "-");
        }

        private static int GetInt([CanBeNull] object obj, int defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }

            if (obj is int)
            {
                return (int)obj;
            }

            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        [CanBeNull]
        private static int[] GetIsoDateParts([NotNull] string isoDate, out bool isUtc)
        {
            isUtc = false;
            if (isoDate.Length != IsoDateLength && isoDate.Length != IsoDateUtcLength && isoDate.Length != IsoDateTimeLength && isoDate.Length != IsoDateTimeUtcLength)
            {
                return null;
            }

            if (Regex.IsMatch(isoDate, @"[^0-9TZ]"))
            {
                return null;
            }

            int[] parts =
            {
                0,
                0,
                0,
                0,
                0,
                0
            };

            parts[0] = GetInt(isoDate.Substring(0, 4), 0);
            parts[1] = GetInt(isoDate.Substring(4, 2), 0);
            parts[2] = GetInt(isoDate.Substring(6, 2), 0);

            if (isoDate.Length > IsoDateLength && isoDate[IsoDateLength] == 'T')
            {
                parts[3] = GetInt(isoDate.Substring(9, 2), 0);
                parts[4] = GetInt(isoDate.Substring(11, 2), 0);
                parts[5] = GetInt(isoDate.Substring(13, 2), 0);
            }

            isUtc = isoDate.EndsWith(IsoDateTimeUtcMarker, StringComparison.InvariantCultureIgnoreCase);

            return parts;
        }

        private static long GetLong([CanBeNull] object obj, long defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }

            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
