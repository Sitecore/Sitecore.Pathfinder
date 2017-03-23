// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.Zip
{
    /// <summary>
    /// Calculates a 32bit Cyclic Redundancy Checksum (CRC) using the
    /// same polynomial used by Zip. This type ie generally not used directly
    /// by applications wishing to create, read, or manipulate zip archive files.
    /// </summary>
    public class CRC32
    {
        private const int BUFFER_SIZE = 8192;

        private int _TotalBytesRead;

        private readonly uint[] crc32Table;

        /// <summary>
        /// Construct an instance of the CRC32 class, pre-initialising the table
        /// for speed of lookup.
        /// </summary>
        public CRC32()
        {
            unchecked
            {
                // This is the official polynomial used by CRC32 in PKZip.
                // Often the polynomial is shown reversed as 0x04C11DB7.
                var dwPolynomial = 0xEDB88320;

                crc32Table = new uint[256];

                for (uint i = 0; i < 256; i++)
                {
                    var dwCrc = i;
                    for (uint j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                        {
                            dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                        }
                        else
                        {
                            dwCrc >>= 1;
                        }
                    }

                    crc32Table[i] = dwCrc;
                }
            }
        }

        /// <summary>
        /// indicates the total number of bytes read on the CRC stream.
        /// This is used when writing the ZipDirEntry when compressing files.
        /// </summary>
        public int TotalBytesRead
        {
            get { return _TotalBytesRead; }
        }

        /// <summary>
        /// Returns the CRC32 for the specified stream.
        /// </summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <returns>the CRC32 calculation</returns>
        public uint GetCrc32([NotNull] Stream input)
        {
            return GetCrc32AndCopy(input, null);
        }

        /// <summary>
        /// Returns the CRC32 for the specified stream, and writes the input into the output stream.
        /// </summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <param name="output">The stream into which to deflate the input</param>
        /// <returns>the CRC32 calculation</returns>
        public uint GetCrc32AndCopy([NotNull] Stream input, [NotNull] Stream output)
        {
            unchecked
            {
                uint crc32Result;
                crc32Result = 0xFFFFFFFF;
                var buffer = new byte[BUFFER_SIZE];
                var readSize = BUFFER_SIZE;

                _TotalBytesRead = 0;
                var count = input.Read(buffer, 0, readSize);
                if (output != null)
                {
                    output.Write(buffer, 0, count);
                }
                _TotalBytesRead += count;
                while (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        crc32Result = (crc32Result >> 8) ^ crc32Table[buffer[i] ^ (crc32Result & 0x000000FF)];
                    }

                    count = input.Read(buffer, 0, readSize);
                    if (output != null)
                    {
                        output.Write(buffer, 0, count);
                    }
                    _TotalBytesRead += count;
                }

                return ~crc32Result;
            }
        }
    }
}
