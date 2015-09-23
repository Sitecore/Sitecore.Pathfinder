// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO
{
    internal class MockedFileSystemService : FileSystemService
    {
        [NotNull]
        public string Contents { get; set; } = string.Empty;

        public override string ReadAllText(string fileName)
        {
            return Contents;
        }
    }
}
