namespace Sitecore.Pathfinder.Extensions.StringExtensions
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Text;
  using System.Text.RegularExpressions;
  using Sitecore.Pathfinder.Diagnostics;

  public static class StringExtensions
  {
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

        if (char.IsWhiteSpace(p))
        {
          chars[n] = char.ToUpper(chars[n]);
        }
      }

      text = new string(chars);

      var result = Regex.Replace(text, @"\W", string.Empty).Replace(@" ", string.Empty);
      if (!char.IsLetter(result[0]))
      {
        result = @"_" + result;
      }

      return result;
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

    [NotNull]
    public static string[] Split([NotNull] this string text, char separator, StringSplitOptions options)
    {
      var s = new[]
      {
        separator
      };

      return text.Split(s, options);
    }

    [NotNull]
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
  }
}
