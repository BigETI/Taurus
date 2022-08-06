using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Taurus.Fragmenters
{
    public abstract class ADefragmenterStream : Stream, IDefragmenterStream
    {
        public override bool CanRead { get; } = false;

        public override bool CanSeek { get; } = false;

        public override bool CanWrite { get; } = true;

        public override int ReadTimeout 
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public abstract bool IsMessagePending { get; }

        public abstract uint AvailableMessageCount { get; }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            throw new NotSupportedException();

        public override void CopyTo(Stream destination, int bufferSize) => throw new NotSupportedException();

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => throw new NotSupportedException();

        public override int EndRead(IAsyncResult asyncResult) => throw new NotSupportedException();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override int Read(Span<byte> buffer) => throw new NotSupportedException();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException();

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public override int ReadByte() => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public abstract bool TryDequeuingMessage(out ReadOnlySpan<byte> message);

        public abstract bool TryDequeuingMessage(Stream outputStream);
    }
}
