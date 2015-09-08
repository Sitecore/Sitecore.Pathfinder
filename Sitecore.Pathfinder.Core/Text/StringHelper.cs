// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Text
{
    public static class StringHelper
    {
        [NotNull]
        public static string UnescapeXmlNodeName([NotNull] string nodeName)
        {
            return nodeName.Replace("--", " ");
        }
    }
}
