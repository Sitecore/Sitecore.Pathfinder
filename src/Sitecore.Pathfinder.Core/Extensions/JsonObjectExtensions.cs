// © 2015 Sitecore Corporation A/S. All rights reserved.

using Newtonsoft.Json.Linq;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class JsonObjectExtensions
    {
        [CanBeNull, ItemNotNull]
        public static JToken ToJToken([NotNull] this string text)
        {
            try
            {
                return JToken.Parse(text);
            }
            catch
            {
                return null;
            }
        }
    }
}
