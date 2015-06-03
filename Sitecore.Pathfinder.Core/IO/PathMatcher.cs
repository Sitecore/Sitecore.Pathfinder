namespace Sitecore.Pathfinder.IO
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Sitecore.Pathfinder.Diagnostics;

  public class PathMatcher
  {
    public PathMatcher([NotNull] string include, [NotNull] string exclude)
    {
      this.Includes = include.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(this.GetRegex).ToList();
      this.Excludes = exclude.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(this.GetRegex).ToList();
    }

    [NotNull]
    protected List<Regex> Excludes { get; }

    [NotNull]
    protected List<Regex> Includes { get; }

    public bool IsMatch([NotNull] string fileName)
    {
      fileName = PathHelper.NormalizeFilePath(fileName);

      return this.Includes.Any(include => include.IsMatch(fileName) && this.Excludes.All(exclude => !exclude.IsMatch(fileName)));
    }

    [NotNull]
    protected Regex GetRegex([NotNull] string wildcard)
    {
      var pattern = '^' + Regex.Escape(wildcard).Replace("/", @"\\").Replace(@"\*\*\\", ".*").Replace(@"\*\*", ".*").Replace(@"\*", @"[^\\]*(\\)?").Replace(@"\?", ".") + '$';
      var options = RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

      return new Regex(pattern, options);
    }
  }
}
