using System;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Zip.Utils
{
    internal class ZipConstants
    {
        [NotNull]
        public static Encoding ZipEncoding
        {
            get { return Encoding.ASCII; }
        }

        public const UInt32 EndOfCentralDirectorySignature = 0x06054b50;

        public const int ZipEntrySignature = 0x04034b50;

        public const int ZipEntryDataDescriptorSignature = 0x08074b50;

        public const int ZipDirEntrySignature = 0x02014b50;
    }
}