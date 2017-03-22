// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Zip.Utils;

namespace Sitecore.Pathfinder.IO.Zip
{
    /// <summary>
    /// ZipEntry class
    /// </summary>
    public class ZipEntry
    {
        private short _bitField;

        private int _compressedSize;

        private short _compressionMethod;

        private int _crc32;

        private byte[] _extra;

        private Stream _fileStream;

        private int _fileStreamFilePosition;

        private int _lastModDateTime;

        private DateTime _lastModified;

        private string _name;

        private int _uncompressedSize;

        private short _versionNeeded;

        private ZipEntry()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is directory.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectory => _name.EndsWith("/");

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [NotNull]
        public string Name => _name;

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size => _uncompressedSize;

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public Stream GetStream()
        {
            // extract a directory to streamwriter?  nothing to do!
            if (_name.EndsWith("/") || _uncompressedSize == 0)
            {
                return Stream.Null;
            }

            _fileStream.Seek(_fileStreamFilePosition, SeekOrigin.Begin);

            Stream source;

            if (_compressionMethod == 0)
            {
                // the System.IO.Compression.DeflateStream class does not handle uncompressed data.
                // so if an entry is not compressed, then we just translate the bytes directly.
                source = _fileStream;
            }
            else
            {
                source = new DeflateStream(_fileStream, CompressionMode.Decompress, true);
            }

            return new LimitedReadOnlyStream(source, _uncompressedSize);
        }

        /// <summary>
        /// Reads one ZipEntry from the given stream.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <param name="encoding">The encoding to use to read entry name.</param>
        /// <returns>the ZipEntry read from the stream.</returns>
        [CanBeNull]
        public static ZipEntry Read([NotNull] Stream stream, [NotNull] Encoding encoding)
        {
            var entry = new ZipEntry();

            if (!ReadHeader(stream, entry, encoding))
            {
                return null;
            }

            entry._fileStreamFilePosition = (int)stream.Position;
            entry._fileStream = stream;

            // finally, seek past the (already read) Data descriptor if necessary
            if ((entry._bitField & 0x0008) == 0x0008)
            {
                stream.Seek(16, SeekOrigin.Current);
            }

            return entry;
        }

        /// <summary>
        /// Finds the signature.
        /// </summary>
        /// <param name="stream">The s.</param>
        /// <param name="signatureToFind">The signature to find.</param>
        /// <returns>The signature.</returns>
        protected internal static long FindSignature([NotNull] Stream stream, int signatureToFind)
        {
            var startingPosition = stream.Position;

            const int BatchSize = 1024;
            var targetBytes = new byte[4];
            targetBytes[0] = (byte)(signatureToFind >> 24);
            targetBytes[1] = (byte)((signatureToFind & 0x00FF0000) >> 16);
            targetBytes[2] = (byte)((signatureToFind & 0x0000FF00) >> 8);
            targetBytes[3] = (byte)(signatureToFind & 0x000000FF);
            var batch = new byte[BatchSize];
            var success = false;
            do
            {
                var n = stream.Read(batch, 0, batch.Length);
                if (n != 0)
                {
                    for (var i = 0; i < n; i++)
                    {
                        if (batch[i] == targetBytes[3])
                        {
                            stream.Seek(i - n, SeekOrigin.Current);
                            var sig = ReadSignature(stream);
                            success = sig == signatureToFind;
                            if (!success)
                            {
                                stream.Seek(-3, SeekOrigin.Current);
                            }
                            break; // out of for loop
                        }
                    }
                }
                else
                {
                    break;
                }

                if (success)
                {
                    break;
                }
            }
            while (true);

            if (!success)
            {
                stream.Seek(startingPosition, SeekOrigin.Begin);
                return -1; // or throw?
            }

            // subtract 4 for the signature.
            var bytesRead = stream.Position - startingPosition - 4;

            // number of bytes read, should be the same as compressed size of file            
            return bytesRead;
        }

        /// <summary>
        /// Reads the signature.
        /// </summary>
        /// <param name="stream">The s.</param>
        /// <returns>The signature.</returns>
        protected internal static int ReadSignature([NotNull] Stream stream)
        {
            var sig = new byte[4];
            var n = stream.Read(sig, 0, sig.Length);
            if (n != sig.Length)
            {
                throw new Exception("Could not read signature - no data!");
            }

            return ((sig[3] * 256 + sig[2]) * 256 + sig[1]) * 256 + sig[0];
        }

        internal long GetNextBlockPosition()
        {
            return _fileStreamFilePosition + _compressedSize + ((_bitField & 8) > 0 ? 12 : 0);
        }

        private static bool ReadHeader([NotNull] Stream stream, [NotNull] ZipEntry entry, [NotNull] Encoding nameEncoding)
        {
            var signature = ReadSignature(stream);

            // Return false if this is not a local file header signature.
            if (signature != ZipConstants.ZipEntrySignature)
            {
                stream.Seek(-4, SeekOrigin.Current); // unread the signature

                // Getting "not a ZipEntry signature" is not always wrong or an error. 
                // This can happen when walking through a zipfile.  After the last compressed entry, 
                // we expect to read a ZipDirEntry signature.  When we get this is how we 
                // know we've reached the end of the compressed entries. 
                if (signature != ZipConstants.ZipDirEntrySignature)
                {
                    throw new Exception(string.Format("  ZipEntry::Read(): Bad signature ({0:X8}) at position  0x{1:X8}", signature, stream.Position));
                }

                return false;
            }

            var block = new byte[26];
            var n = stream.Read(block, 0, block.Length);
            if (n != block.Length)
            {
                return false;
            }

            var i = 0;
            entry._versionNeeded = (short)(block[i++] + block[i++] * 256);
            entry._bitField = (short)(block[i++] + block[i++] * 256);
            entry._compressionMethod = (short)(block[i++] + block[i++] * 256);
            entry._lastModDateTime = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;

            // the PKZIP spec says that if bit 3 is set (0x0008), then the CRC, Compressed size, and uncompressed size
            // come directly after the file data.  The only way to find it is to scan the zip archive for the signature of 
            // the Data Descriptor, and presume that that signature does not appear in the (compressed) data of the compressed file.  

            if ((entry._bitField & 0x0008) != 0x0008)
            {
                entry._crc32 = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
                entry._compressedSize = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
                entry._uncompressedSize = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            }
            else
            {
                // the CRC, compressed size, and uncompressed size are stored later in the stream.
                // here, we advance the pointer.
                i += 12;
            }

            var filenameLength = (short)(block[i++] + block[i++] * 256);
            var extraFieldLength = (short)(block[i++] + block[i++] * 256);

            block = new byte[filenameLength];
            stream.Read(block, 0, block.Length);
            entry._name = StringFromBuffer(block, block.Length, nameEncoding);

            entry._extra = new byte[extraFieldLength];
            stream.Read(entry._extra, 0, entry._extra.Length);

            // transform the time data into something usable
            entry._lastModified = UnpackDateTime(entry._lastModDateTime);

            // actually get the compressed size and CRC if necessary
            if ((entry._bitField & 0x0008) == 0x0008)
            {
                var posn = stream.Position;
                var sizeOfDataRead = FindSignature(stream, ZipConstants.ZipEntryDataDescriptorSignature);
                if (sizeOfDataRead == -1)
                {
                    return false;
                }

                // read 3x 4-byte fields (CRC, Compressed Size, Uncompressed Size)
                block = new byte[12];
                n = stream.Read(block, 0, block.Length);
                if (n != 12)
                {
                    return false;
                }

                i = 0;
                entry._crc32 = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
                entry._compressedSize = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
                entry._uncompressedSize = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;

                if (sizeOfDataRead != entry._compressedSize)
                {
                    throw new Exception("Data format error (bit 3 is set)");
                }

                // seek back to previous position, to read file data
                stream.Seek(posn, SeekOrigin.Begin);
            }

            return true;
        }

        [NotNull]
        private static string StringFromBuffer([NotNull] byte[] buf, int maxlength, [NotNull] Encoding encoding)
        {
            if (maxlength > buf.Length)
            {
                maxlength = buf.Length;
            }

            var length = Array.IndexOf(buf, 0, 0, maxlength);
            if (length < 0)
            {
                length = maxlength;
            }

            return encoding.GetString(buf, 0, length);
        }

        private static DateTime UnpackDateTime(int packedDateTime)
        {
            var num1 = (short)(packedDateTime & ushort.MaxValue);
            var num2 = (short)((packedDateTime & 4294901760L) >> 16);
            var year = 1980 + ((num2 & 65024) >> 9);
            var month = (num2 & 480) >> 5;
            var day = num2 & 31;
            var hour = (num1 & 63488) >> 11;
            var minute = (num1 & 2016) >> 5;
            var second = num1 & 31;
            try
            {
                return new DateTime(year, month, day, hour, minute, second, 0);
            }
            catch
            {
                return DateTime.Now;
            }
        }
    }
}
