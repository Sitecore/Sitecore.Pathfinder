using System;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Zip.Utils
{
    /// <summary>
    /// Calculates a 32bit Cyclic Redundancy Checksum (CRC) using the
    /// same polynomial used by Zip. This type ie generally not used directly
    /// by applications wishing to create, read, or manipulate zip archive files.
    /// </summary>
    public class CRC32
    {
        UInt32[] crc32Table;

        const int BUFFER_SIZE = 8192;

        /// <summary>
        /// indicates the total number of bytes read on the CRC stream.
        /// This is used when writing the ZipDirEntry when compressing files.
        /// </summary>
        public Int32 TotalBytesRead
        {
            get { return _TotalBytesRead; }
        }

        /// <summary>
        /// Returns the CRC32 for the specified stream.
        /// </summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <returns>the CRC32 calculation</returns>
        public UInt32 GetCrc32([NotNull] Stream input)
        {
            return GetCrc32AndCopy(input, null);
        }

        /// <summary>
        /// Returns the CRC32 for the specified stream, and writes the input into the output stream.
        /// </summary>
        /// <param name="input">The stream over which to calculate the CRC32</param>
        /// <param name="output">The stream into which to deflate the input</param>
        /// <returns>the CRC32 calculation</returns>
        public UInt32 GetCrc32AndCopy([NotNull] Stream input, [NotNull] Stream output)
        {
            unchecked
            {
                UInt32 crc32Result;
                crc32Result = 0xFFFFFFFF;
                byte[] buffer = new byte[BUFFER_SIZE];
                int readSize = BUFFER_SIZE;

                _TotalBytesRead = 0;
                int count = input.Read(buffer, 0, readSize);
                if (output != null)
                    output.Write(buffer, 0, count);
                _TotalBytesRead += count;
                while (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        crc32Result = ((crc32Result) >> 8) ^ crc32Table[(buffer[i]) ^ ((crc32Result) & 0x000000FF)];
                    }

                    count = input.Read(buffer, 0, readSize);
                    if (output != null)
                        output.Write(buffer, 0, count);
                    _TotalBytesRead += count;
                }

                return ~crc32Result;
            }
        }


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
                UInt32 dwPolynomial = 0xEDB88320;

                crc32Table = new UInt32[256];

                for (UInt32 i = 0; i < 256; i++)
                {
                    UInt32 dwCrc = i;
                    for (UInt32 j = 8; j > 0; j--)
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

        private Int32 _TotalBytesRead = 0;
    }
}