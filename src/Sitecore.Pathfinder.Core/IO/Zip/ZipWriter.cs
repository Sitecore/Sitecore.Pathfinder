// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.Zip
{
    /// <summary>
    /// Zip builder class
    /// </summary>
    public class ZipWriter : IDisposable
    {
        private readonly Encoding _encoding;

        private readonly string _tempFileName;

        private int _entriesCount;

        private Stream _outputStream;

        private Stream _tempFileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipWriter"/> class.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        public ZipWriter([NotNull] string outputFile) : this(outputFile, ZipConstants.ZipEncoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipWriter"/> class. Uses given encoding for file names.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        /// <param name="encoding">The encoding.</param>
        public ZipWriter([NotNull] string outputFile, [NotNull] Encoding encoding)
        {
            try
            {
                _outputStream = File.Create(outputFile);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Cannot create file '{0}'", outputFile), e);
            }

            _encoding = encoding;
            _tempFileName = Path.GetTempFileName();
            _tempFileStream = new FileStream(_tempFileName, FileMode.OpenOrCreate);
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="entryName">Name of the entry.</param>
        /// <param name="filePath">The file path.</param>
        public void AddEntry([NotNull] string entryName, [NotNull] string filePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                AddEntry(entryName, fs);
            }
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="entryName">Name of the entry.</param>
        /// <param name="bytes">The bytes.</param>
        public void AddEntry([NotNull] string entryName, [NotNull] byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                AddEntry(entryName, ms);
            }
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="entryName">Name of the entry.</param>
        /// <param name="stream">The stream.</param>
        public void AddEntry([NotNull] string entryName, [NotNull] Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream should be readable");
            }

            // To do: assert whether entry name is unique and valid

            var entryInfo = new ZipEntryInfo();
            entryInfo.EntryName = entryName;
            entryInfo.LastModifiedDate = DateTime.Now - new TimeSpan(1, 0, 0);
            Archive(stream, _outputStream, _tempFileStream, entryInfo, _encoding);
            _entriesCount++;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_outputStream == null)
            {
                return;
            }

            WriteZipFooter();

            // Closing file
            _outputStream.Flush();
            _outputStream.Dispose();
            _outputStream = null;

            // Deleting temporary file
            _tempFileStream.Flush();
            _tempFileStream.Dispose();
            _tempFileStream = null;

            File.Delete(_tempFileName);
        }

        internal static void WriteCentralDirectoryFooter([NotNull] Stream output, long startOfCentralDirectory, long endOfCentralDirectory, int entriesCount)
        {
            var bytes = new byte[1024];
            var i = 0;

            // signature
            bytes[i++] = (byte)(ZipConstants.EndOfCentralDirectorySignature & 0x000000FF);
            bytes[i++] = (byte)((ZipConstants.EndOfCentralDirectorySignature & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((ZipConstants.EndOfCentralDirectorySignature & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((ZipConstants.EndOfCentralDirectorySignature & 0xFF000000) >> 24);

            // number of this disk
            bytes[i++] = 0;
            bytes[i++] = 0;

            // number of the disk with the start of the central directory
            bytes[i++] = 0;
            bytes[i++] = 0;

            // total number of entries in the central dir on this disk
            bytes[i++] = (byte)(entriesCount & 0x00FF);
            bytes[i++] = (byte)((entriesCount & 0xFF00) >> 8);

            // total number of entries in the central directory
            bytes[i++] = (byte)(entriesCount & 0x00FF);
            bytes[i++] = (byte)((entriesCount & 0xFF00) >> 8);

            // size of the central directory
            var sizeOfCentralDirectory = (int)(endOfCentralDirectory - startOfCentralDirectory);
            bytes[i++] = (byte)(sizeOfCentralDirectory & 0x000000FF);
            bytes[i++] = (byte)((sizeOfCentralDirectory & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((sizeOfCentralDirectory & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((sizeOfCentralDirectory & 0xFF000000) >> 24);

            // offset of the start of the central directory 
            var startOffset = (int)startOfCentralDirectory; // cast down from Long
            bytes[i++] = (byte)(startOffset & 0x000000FF);
            bytes[i++] = (byte)((startOffset & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((startOffset & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((startOffset & 0xFF000000) >> 24);

            // zip comment length
            bytes[i++] = 0;
            bytes[i++] = 0;

            output.Write(bytes, 0, i);
        }

        private static void Archive([NotNull] Stream input, [NotNull] Stream output, [NotNull] Stream headerOutput, [NotNull] ZipEntryInfo additionalInfo, [NotNull] Encoding encoding)
        {
            var bytes = new byte[4096];

            long i = 0;

            // signature
            Write4Bytes(bytes, ref i, ZipConstants.ZipEntrySignature);

            // version needed
            short fixedVersionNeeded = 0x14; // from examining existing zip files
            Write2Bytes(bytes, ref i, fixedVersionNeeded);

            // bitfield
            short bitField = 0x00; // from examining existing zip files
            Write2Bytes(bytes, ref i, bitField);

            // compression method         
            short compressionMethod = 0x08; // 0x08 = Deflate, 0x00 == No Compression
            Write2Bytes(bytes, ref i, compressionMethod);

            // LastMod
            var lastModDateTime = additionalInfo.LastModifiedDateInt;
            Write4Bytes(bytes, ref i, lastModDateTime);

            // Writing zeros for Crc32, CompressedSize and UncompressedSize. Total length is 12 bytes
            // This fields will be filled later, after compression
            var offsetOfCrcAndSizesInHeader = i;
            var offsetOfCrcAndSizes = output.Position + i;

            i += 12; // Increasing offset by 12. 

            // filename length (Int16)
            var data = encoding.GetBytes(additionalInfo.EntryName);
            var length = (short)data.Length;
            Write2Bytes(bytes, ref i, length);

            // extra field length (short)
            short extraFieldLength = 0x00;
            Write2Bytes(bytes, ref i, extraFieldLength);

            // actual filename
            Array.Copy(data, 0, bytes, (int)i, data.Length);
            i += data.Length;

            // remember the file offset of this header
            long relativeOffsetOfHeader = (int)output.Position;

            // finally, write the header to the stream
            output.Write(bytes, 0, (int)i);

            var crc32 = new CRC32();
            uint crc;
            var fileDataOffset = output.Position;
            using (var compressedStream = new DeflateStream(output, CompressionMode.Compress, true))
            {
                crc = crc32.GetCrc32AndCopy(input, compressedStream);
            }

            var compressedSize = output.Position - fileDataOffset;
            long originalSize = crc32.TotalBytesRead;

            //// Checking whether input stream is empty
            //if(originalSize == 0)
            //{
            //   SetNoCompression(output, offsetOfCompression);

            //   compressedSize = 0;
            //   crc = 0;
            //   output.Seek(fileDataOffset, SeekOrigin.Begin);
            //}

            // We don't need the input stream, so we close it
            input.Dispose();

            WriteCrcAndSizes(output, offsetOfCrcAndSizes, crc, originalSize, compressedSize);

            // Copying crc, originalSize, compressedSize to the bytes. In order to be used in central directory structure
            var k = offsetOfCrcAndSizesInHeader;

            Write4Bytes(bytes, ref k, (int)crc);
            Write4Bytes(bytes, ref k, (int)compressedSize);
            Write4Bytes(bytes, ref k, (int)originalSize);

            var header = new byte[i];
            Array.Copy(bytes, header, (int)i);

            WriteCenralDirectoryFileHeader(headerOutput, header, relativeOffsetOfHeader);
        }

        private static int PackDateTime(DateTime time)
        {
            return ((ushort)((time.Day & 31) | ((time.Month << 5) & 480) | (((time.Year - 1980) << 9) & 65024)) << 16) | (ushort)((time.Second & 31) | ((time.Minute << 5) & 2016) | ((time.Hour << 11) & 63488));
        }

        private static void Write2Bytes([NotNull] byte[] array, ref long index, short value)
        {
            array[index++] = (byte)(value & 0x000000FF);
            array[index++] = (byte)((value & 0x0000FF00) >> 8);
        }

        private static void Write4Bytes([NotNull] byte[] array, ref long index, long value)
        {
            array[index++] = (byte)(value & 0x000000FF);
            array[index++] = (byte)((value & 0x0000FF00) >> 8);
            array[index++] = (byte)((value & 0x00FF0000) >> 16);
            array[index++] = (byte)((value & 0xFF000000) >> 24);
        }

        private static void WriteCenralDirectoryFileHeader([NotNull] Stream output, [NotNull] byte[] localHeader, long offsetOfLocalHeader)
        {
            var bytes = new byte[4096];
            long i = 0;

            // signature
            Write4Bytes(bytes, ref i, ZipConstants.ZipDirEntrySignature);

            // Version Made By
            bytes[i++] = localHeader[4];
            bytes[i++] = localHeader[5];

            // Version Needed, Bitfield, compression method, lastmod,
            // crc, sizes, filename length and extra field length -
            // are all the same as the local file header. So just copy them
            int j;
            for (j = 0; j < 26; j++)
            {
                bytes[i + j] = localHeader[4 + j];
            }

            i += j; // positioned at next available byte

            // File Comment Length
            Write2Bytes(bytes, ref i, 0);

            // Disk number start
            Write2Bytes(bytes, ref i, 0);

            // internal file attrs
            // TODO: figure out what is required here. 
            Write2Bytes(bytes, ref i, 1);

            // external file attrs
            // TODO: figure out what is required here. 
            Write4Bytes(bytes, ref i, 0x81b60020);

            // relative offset of local header (I think this can be zero)
            Write4Bytes(bytes, ref i, (int)offsetOfLocalHeader);

            // actual filename (starts at offset 34 in header) 
            for (j = 0; j < localHeader.Length - 30; j++)
            {
                bytes[i + j] = localHeader[30 + j];
            }

            i += j;

            output.Write(bytes, 0, (int)i);
        }

        private static void WriteCrcAndSizes([NotNull] Stream output, long offset, long crc32, long originalSize, long compSize)
        {
            var bytes = new byte[12];
            var i = 0;

            // calculated above
            bytes[i++] = (byte)(crc32 & 0x000000FF);
            bytes[i++] = (byte)((crc32 & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((crc32 & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((crc32 & 0xFF000000) >> 24);
            bytes[i++] = (byte)(compSize & 0x000000FF);
            bytes[i++] = (byte)((compSize & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((compSize & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((compSize & 0xFF000000) >> 24);
            bytes[i++] = (byte)(originalSize & 0x000000FF);
            bytes[i++] = (byte)((originalSize & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((originalSize & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((originalSize & 0xFF000000) >> 24);

            var pos = output.Position;
            output.Seek(offset, SeekOrigin.Begin);
            output.Write(bytes, 0, 12);
            output.Seek(pos, SeekOrigin.Begin);
        }

        private void WriteZipFooter()
        {
            var centralDirectoryOffsetBegin = _outputStream.Position;

            // Copy entry headers from temp file stream
            _tempFileStream.Seek(0, SeekOrigin.Begin);
            _tempFileStream.CopyTo(_outputStream);

            var centralDirectoryOffsetEnd = _outputStream.Position;

            WriteCentralDirectoryFooter(_outputStream, centralDirectoryOffsetBegin, centralDirectoryOffsetEnd, _entriesCount);

            // Shrinking file
            _outputStream.SetLength(_outputStream.Position);
        }

        private class ZipEntryInfo
        {
            [NotNull]
            public string EntryName = string.Empty;

            public DateTime LastModifiedDate = DateTime.Now;

            public int LastModifiedDateInt => PackDateTime(LastModifiedDate);
        }
    }
}
