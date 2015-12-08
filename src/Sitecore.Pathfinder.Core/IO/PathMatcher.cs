// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    public class PathMatcher
    {
        public PathMatcher([NotNull] string include, [NotNull] string exclude)
        {
            Includes = include.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(GetRegex).ToList();
            Excludes = exclude.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(GetRegex).ToList();
        }

        [NotNull, ItemNotNull]
        protected List<Regex> Excludes { get; }

        [NotNull, ItemNotNull]
        protected List<Regex> Includes { get; }

        public bool IsMatch([NotNull] string fileName)
        {
            fileName = PathHelper.NormalizeFilePath(fileName);

            return Includes.Any(include => include.IsMatch(fileName) && Excludes.All(exclude => !exclude.IsMatch(fileName)));
        }

        [NotNull]
        protected Regex GetRegex([NotNull] string wildcard)
        {
            // todo: consider caching this
            var pattern = '^' + Regex.Escape(wildcard).Replace("/", @"\\").Replace(@"\*\*\\", ".*").Replace(@"\*\*", ".*").Replace(@"\*", @"[^\\]*(\\)?").Replace(@"\?", ".") + '$';
            var options = RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

            return new Regex(pattern, options);
        }
    }
}
