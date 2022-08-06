using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Taurus.Fragmenters
{
    public interface IStream : IAsyncDisposable, IDisposable
    {
        bool CanRead { get; }

        bool CanSeek { get; }

        bool CanTimeout { get; }

        bool CanWrite { get; }

        long Length { get; }

        long Position { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }


        IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state);

        IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state);

        void Close();

        void CopyTo(Stream destination);

        void CopyTo(Stream destination, int bufferSize);

        Task CopyToAsync(Stream destination);

        Task CopyToAsync(Stream destination, int bufferSize);

        Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken);

        Task CopyToAsync(Stream destination, CancellationToken cancellationToken);

        int EndRead(IAsyncResult asyncResult);

        void EndWrite(IAsyncResult asyncResult);

        void Flush();

        Task FlushAsync();

        Task FlushAsync(CancellationToken cancellationToken);

        int Read(byte[] buffer, int offset, int count);

        int Read(Span<byte> buffer);

        Task<int> ReadAsync(byte[] buffer, int offset, int count);

        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        int ReadByte();

        long Seek(long offset, SeekOrigin origin);

        void SetLength(long value);

        void Write(byte[] buffer, int offset, int count);

        void Write(ReadOnlySpan<byte> buffer);

        Task WriteAsync(byte[] buffer, int offset, int count);

        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

        void WriteByte(byte value);
    }
}
