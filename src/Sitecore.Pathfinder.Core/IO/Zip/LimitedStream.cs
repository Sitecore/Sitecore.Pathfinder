using System;
using System.IO;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Zip.Utils
{
    class LimitedReadOnlyStream : Stream
    {
        [NotNull]
        readonly Stream _innerStream;

        readonly long _limit = 0;

        long _totalBytesRead = 0;

        public LimitedReadOnlyStream([NotNull] Stream innerStream, int limit)
        {
            _innerStream = innerStream;
            _limit = limit;
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush()
        {
        }

        public override long Length => _limit;

        public override long Position
        {
            get { throw new Exception("The method or operation are not accessible."); }
            set { throw new Exception("The method or operation are not accessible."); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new Exception("You cannot read this stream");

            if (_totalBytesRead == _limit)
                return 0;

            count = (int)Math.Min(_limit - _totalBytesRead, count);
            int bytesRead = _innerStream.Read(buffer, offset, count);
            _totalBytesRead += bytesRead;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new Exception("Operation is not supported.");
        }

        public override void SetLength(long value)
        {
            throw new Exception("Operation is not supported.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("Operation is not supported.");
        }
    }
}