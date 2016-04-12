// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Configuration;
using Sitecore.IO;

namespace Sitecore.Pathfinder
{
    public static class WebsiteHost
    {
        [CanBeNull]
        private static IAppService _app;

        [CanBeNull]
        public static IAppService App => _app ?? (_app = new Startup().WithToolsDirectory(FileUtil.MapPath("/bin")).WithProjectDirectory(FileUtil.MapPath("/")).WithWebsiteDirectory(FileUtil.MapPath("/")).WithDataFolderDirectory(FileUtil.MapPath(Settings.DataFolder)).DoNotLoadConfigFiles().Start());
    }
}
