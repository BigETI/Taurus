using System;
using System.IO;

/// <summary>
/// Taurus fragmenters namespace
/// </summary>
namespace Taurus.Fragmenters
{
    /// <summary>
    /// An interface that represents a defragmener stream
    /// </summary>
    public interface IDefragmenterStream : IStream
    {
        /// <summary>
        /// Is message pending
        /// </summary>
        bool IsMessagePending { get; }

        /// <summary>
        /// Available message count
        /// </summary>
        uint AvailableMessageCount { get; }

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        bool TryDequeuingMessage(out ReadOnlySpan<byte> message);

        /// <summary>
        /// Tries to dequeue a message
        /// </summary>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" is message has been successfully dequeued, otherwise "false"</returns>
        bool TryDequeuingMessage(Stream outputStream);
    }
}
