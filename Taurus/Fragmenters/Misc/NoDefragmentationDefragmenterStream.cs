using System;
using System.Collections.Generic;
using System.IO;

namespace Taurus.Fragmenters
{
    /// <summary>
    /// A class that describes a no defragmentation defragmenter
    /// </summary>
    internal sealed class NoDefragmentationDefragmenterStream : ADefragmenterStream, INoDefragmentationDefragmenterStream
    {
        /// <summary>
        /// Message queue
        /// </summary>
        private readonly Queue<ReadOnlyMemory<byte>> messageQueue = new Queue<ReadOnlyMemory<byte>>();

        /// <summary>
        /// Is message pending
        /// </summary>
        public override bool IsMessagePending { get; } = false;

        /// <summary>
        /// Available message count
        /// </summary>
        public override uint AvailableMessageCount { get; } = 0U;

        /// <summary>
        /// Flushes this stream
        /// </summary>
        public override void Flush()
        {
            // ...
        }

        /// <summary>
        /// Writes data into this stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        public override void Write(byte[] buffer, int offset, int count) =>
            messageQueue.Enqueue(buffer.AsMemory(offset, count));

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public override bool TryDequeuingMessage(out ReadOnlySpan<byte> message)
        {
            bool ret = messageQueue.TryDequeue(out ReadOnlyMemory<byte> bytes);
            message = ret ? bytes.Span : ReadOnlySpan<byte>.Empty;
            return ret;
        }

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public override bool TryDequeuingMessage(Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write contents to output stream.", nameof(outputStream));
            }
            bool ret = messageQueue.TryDequeue(out ReadOnlyMemory<byte> bytes);
            if (ret)
            {
                outputStream.Write(bytes.Span);
            }
            return ret;
        }
    }
}
