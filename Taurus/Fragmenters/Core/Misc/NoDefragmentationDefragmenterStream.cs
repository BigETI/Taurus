using System;
using System.IO;

/// <summary>
/// Taurus fragmenters namespace
/// </summary>
namespace Taurus.Fragmenters
{
    /// <summary>
    /// A class that describes a no defragmentation defragmenter
    /// </summary>
    internal class NoDefragmentationDefragmenterStream : ADefragmenterStream, INoDefragmentationDefragmenterStream
    {
        /// <summary>
        /// Memory stream
        /// </summary>
        private readonly MemoryStream memoryStream = new MemoryStream();

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
        public override void Flush() => memoryStream.Flush();

        /// <summary>
        /// Writes data into thi stream
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        public override void Write(byte[] buffer, int offset, int count) => memoryStream.Write(buffer, offset, count);

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        public override bool TryDequeuingMessage(out ReadOnlySpan<byte> message)
        {
            bool ret = memoryStream.Length > 0;
            if (ret)
            {
                memoryStream.Seek(0L, SeekOrigin.Begin);
                message = memoryStream.ToArray();
            }
            else
            {
                message = Array.Empty<byte>();
            }
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
            bool ret = memoryStream.Length > 0L;
            if (ret)
            {
                memoryStream.CopyTo(outputStream);
                memoryStream.SetLength(0L);
            }
            return ret;
        }
    }
}
