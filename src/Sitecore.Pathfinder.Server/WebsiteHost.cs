// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Configuration;
using Sitecore.IO;

namespace Sitecore.Pathfinder
{
    public static class WebsiteHost
    {
        [CanBeNull]
        private static IHostService _host;

        [CanBeNull]
        public static IHostService Host => _host ?? (_host = new Startup().WithToolsDirectory(FileUtil.MapPath("/bin")).WithProjectDirectory(FileUtil.MapPath("/")).WithWebsiteDirectory(FileUtil.MapPath("/")).WithDataFolderDirectory(FileUtil.MapPath(Settings.DataFolder)).DoNotLoadConfigFiles().Start());
    }
}
