// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Configuration;

namespace Sitecore.Pathfinder.Configuration
{
    public static class PathfinderSettings
    {
        [Diagnostics.NotNull]
        public static string HtmlTemplateExtension => Settings.GetSetting("Pathfinder.HtmlTemplateExtension", ".html");
    }
}
