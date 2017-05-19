// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class GuidExtensions
    {
        [NotNull]
        public static string Format(this Guid guid)
        {
            return guid.ToString("B").ToUpperInvariant();
        }

        public static bool IsGuid([NotNull] this string s)
        {
            Guid g;
            return Guid.TryParse(s, out g);
        }
        public static bool IsGuidOrSoftGuid([NotNull] this string s)
        {
            return s.StartsWith("{") && s.EndsWith("}");
        }
    }
}
