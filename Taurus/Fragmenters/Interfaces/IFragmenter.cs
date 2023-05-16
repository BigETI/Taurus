using System;
using System.IO;

namespace Taurus.Fragmenters
{
    /// <summary>
    /// A class that describes a fragmenter
    /// </summary>
    public interface IFragmenter
    {
        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Fragmented message</returns>
        ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> message);

        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="outputStream">Output stream</param>
        void Fragment(ReadOnlySpan<byte> message, Stream outputStream);

        /// <summary>
        /// Creates a new defragmenter stream
        /// </summary>
        /// <returns>Defragmenter stream</returns>
        IDefragmenterStream CreateNewDefragmenterStream();
    }
}
