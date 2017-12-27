// © 2017 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Languages.Webdeploy.Cloud
{
    public class Blob
    {
        [NotNull]
        public string Database { get; set; }

        [NotNull]
        public string Id { get; set; }

        [NotNull]
        public byte[] Data { get; set; }
    }
}
