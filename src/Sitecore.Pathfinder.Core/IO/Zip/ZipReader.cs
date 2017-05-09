// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.IO.Zip
{
    /// <summary>
    /// ZipReader class
    /// </summary>
    public class ZipReader : IDisposable
    {
        [NotNull]
        private readonly Encoding _encoding;

        [NotNull]
        private Stream _inputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipReader"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public ZipReader([NotNull] Stream input) : this(input, ZipConstants.ZipEncoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipReader"/> class. Uses given encoding to read names of archived files.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The encoding.</param>
        public ZipReader([NotNull] Stream input, [NotNull] Encoding encoding)
        {
            if (!input.CanRead)
            {
                throw new ArgumentException("Stream should be readable", nameof(input));
            }

            _inputStream = input;
            _encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipReader"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public ZipReader([NotNull] string filename) : this(filename, ZipConstants.ZipEncoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipReader"/> class.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="encoding">The encoding.</param>
        public ZipReader([NotNull] string filename, [NotNull] Encoding encoding)
        {
            try
            {
                _inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while opening file '{0}'", filename), e);
            }

            _encoding = encoding;
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>The entries.</value>
        [ItemNotNull, NotNull]
        public IEnumerable<ZipEntry> Entries
        {
            get
            {
                long pos = 0;
                while (true)
                {
                    _inputStream.Seek(pos, SeekOrigin.Begin);
                    var entry = ZipEntry.Read(_inputStream, _encoding);
                    if (entry == null)
                    {
                        break;
                    }

                    pos = entry.GetNextBlockPosition();

                    yield return entry;
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_inputStream != null)
            {
                _inputStream.Dispose();
                _inputStream = null;
            }
        }

        #endregion

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="entryName">Name of the entry.</param>
        /// <returns></returns>
        [CanBeNull]
        public ZipEntry GetEntry([NotNull] string entryName)
        {
            foreach (var entry in Entries)
            {
                if (entry.Name == entryName)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
