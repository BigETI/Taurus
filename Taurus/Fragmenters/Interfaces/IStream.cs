using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Taurus.Fragmenters
{
    /// <summary>
    /// An interface that represents a stream
    /// </summary>
    public interface IStream : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Can read from this stream
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Can seek inside this stream
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// Can this stream timeout
        /// </summary>
        bool CanTimeout { get; }

        /// <summary>
        /// Can write into this stream
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Length of this stream
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Position inside this stream
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Read timeout
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Write timeout
        /// </summary>
        int WriteTimeout { get; set; }

        /// <summary>
        /// Begins to read from stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <param name="callback">Callback</param>
        /// <param name="state">State</param>
        /// <returns>Read from this stream asynchronous result</returns>
        IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state);

        /// <summary>
        /// Begins to write into this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <param name="callback">Callback</param>
        /// <param name="state">State</param>
        /// <returns>Write into this stream asynchronous result</returns>
        IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state);

        /// <summary>
        /// Closes this stream
        /// </summary>
        void Close();

        /// <summary>
        /// Copies data to the specified stream
        /// </summary>
        /// <param name="destination">Destination stream</param>
        void CopyTo(Stream destination);

        /// <summary>
        /// Copies data to the specified stream
        /// </summary>
        /// <param name="destination">Destination stream</param>
        /// <param name="bufferSize">Buffer size</param>
        void CopyTo(Stream destination, int bufferSize);

        /// <summary>
        /// COpies data to the specified stream asynchronously
        /// </summary>
        /// <param name="destination">Destination</param>
        /// <returns>Task</returns>
        Task CopyToAsync(Stream destination);

        /// <summary>
        /// Copies data to the specified stream
        /// </summary>
        /// <param name="destination">Destination stream</param>
        /// <param name="bufferSize">Buffer size</param>
        /// <returns>Task</returns>
        Task CopyToAsync(Stream destination, int bufferSize);

        /// <summary>
        /// Copies data to the specified stream asynchronously
        /// </summary>
        /// <param name="destination">Destination stream</param>
        /// <param name="bufferSize">Buffer size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken);

        /// <summary>
        /// Copies data to the specified stream asynchronously
        /// </summary>
        /// <param name="destination">Destination stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task CopyToAsync(Stream destination, CancellationToken cancellationToken);

        /// <summary>
        /// Ends reading from this stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        /// <returns>Number of bytes read</returns>
        int EndRead(IAsyncResult asyncResult);

        /// <summary>
        /// Ends writing into this stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        void EndWrite(IAsyncResult asyncResult);

        /// <summary>
        /// Flushes this stream
        /// </summary>
        void Flush();

        /// <summary>
        /// Flushes this stream asynchronously
        /// </summary>
        /// <returns>Task</returns>
        Task FlushAsync();

        /// <summary>
        /// Flushes this stream asynchronously
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task FlushAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Reads data from this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <returns>Number of bytes read</returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Reads data from this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Number of bytes read</returns>
        int Read(Span<byte> buffer);

        /// <summary>
        /// Reads data from this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <returns>Number of bytes read task</returns>
        Task<int> ReadAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// Reads data from this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of bytes read task</returns>
        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        /// <summary>
        /// Reads data from this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of bytes read task</returns>
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads a single byte from this stream
        /// </summary>
        /// <returns>Byte if sucessful, otherwise "-1"</returns>
        int ReadByte();

        /// <summary>
        /// Seeks within this stream
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="origin">Origin</param>
        /// <returns>New position inside this stream</returns>
        long Seek(long offset, SeekOrigin origin);

        /// <summary>
        /// Sets the length of this stream
        /// </summary>
        /// <param name="value">New length of this stream</param>
        void SetLength(long value);

        /// <summary>
        /// Writes data into this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes data into this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        void Write(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Writes data into this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <returns>Task</returns>
        Task WriteAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes data into this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        /// <summary>
        /// Writes data into this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes a single byte into this stream
        /// </summary>
        /// <param name="value">Byte</param>
        void WriteByte(byte value);
    }
}
