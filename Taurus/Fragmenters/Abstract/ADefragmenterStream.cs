using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Taurus.Fragmenters
{
    /// <summary>
    /// An abstract class that describes a defragmenter stream
    /// </summary>
    public abstract class ADefragmenterStream : Stream, IDefragmenterStream
    {
        /// <summary>
        /// Can read from this stream
        /// </summary>
        public override bool CanRead { get; } = false;

        /// <summary>
        /// Can seek inside this stream
        /// </summary>
        public override bool CanSeek { get; } = false;

        /// <summary>
        /// Can write into this stream
        /// </summary>
        public override bool CanWrite { get; } = true;

        /// <summary>
        /// Read timeout
        /// </summary>
        /// <exception cref="NotSupportedException">This property is not supported</exception>
        public override int ReadTimeout
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Length of this stream
        /// </summary>
        /// <exception cref="NotSupportedException">This property is not supported</exception>
        public override long Length => throw new NotSupportedException();

        /// <summary>
        /// Position inside this stream
        /// </summary>
        /// <exception cref="NotSupportedException">This property is not supported</exception>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Is message pending
        /// </summary>
        public abstract bool IsMessagePending { get; }

        /// <summary>
        /// Available message count
        /// </summary>
        public abstract uint AvailableMessageCount { get; }

        /// <summary>
        /// Begins to read from stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <param name="callback">Callback</param>
        /// <param name="state">State</param>
        /// <returns>Read from this stream asynchronous result</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            throw new NotSupportedException();

        /// <summary>
        /// Copies data to the specified stream
        /// </summary>
        /// <param name="destination">Destination stream</param>
        /// <param name="bufferSize">Buffer size</param>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override void CopyTo(Stream destination, int bufferSize) => throw new NotSupportedException();

        /// <summary>
        /// Copies data to the specified stream asynchronously
        /// </summary>
        /// <param name="destination">Destination stream</param>
        /// <param name="bufferSize">Buffer size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => throw new NotSupportedException();

        /// <summary>
        /// Ends reading from this stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        /// <returns>Number of bytes read</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override int EndRead(IAsyncResult asyncResult) => throw new NotSupportedException();

        /// <summary>
        /// Reads data from this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <returns>Number of bytes read</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <summary>
        /// Reads data from this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Number of bytes read</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override int Read(Span<byte> buffer) => throw new NotSupportedException();

        /// <summary>
        /// Reads data from this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of bytes read task</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException();

        /// <summary>
        /// Reads data from this stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of bytes read task</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        /// <summary>
        /// Reads a single byte from this stream
        /// </summary>
        /// <returns>Byte if sucessful, otherwise "-1"</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override int ReadByte() => throw new NotSupportedException();

        /// <summary>
        /// Seeks within this stream
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="origin">Origin</param>
        /// <returns>New position inside this stream</returns>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <summary>
        /// Sets the length of this stream
        /// </summary>
        /// <param name="value">New length of this stream</param>
        /// <exception cref="NotSupportedException">This method is not supported</exception>
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public abstract bool TryDequeuingMessage(out ReadOnlySpan<byte> message);

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public abstract bool TryDequeuingMessage(Stream outputStream);
    }
}
