// © 2015 Sitecore Corporation A/S. All rights reserved.

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

        public static bool IsGuid(this string guid)
        {
            Guid g;
            return Guid.TryParse(guid, out g);
        }
    }
}
