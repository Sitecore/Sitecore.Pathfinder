// © 2015-2017 by Jakob Christensen. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public class PackageFile
    {
        [NotNull]
        public byte[] Content { get; set; }

        [NotNull]
        public string FileName { get; set; }
    }
}
