using System;
using System.IO;

/// <summary>
/// Taurus fragmenters namespace
/// </summary>
namespace Taurus.Fragmenters
{
    /// <summary>
    /// A class that describes a fragmenter
    /// </summary>
    public abstract class AFragmenter : IFragmenter
    {
        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Fragmented bytes</returns>
        public abstract ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> message);

        /// <summary>
        /// Fragments the specfied message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="outputStream">Output stream</param>
        public abstract void Fragment(ReadOnlySpan<byte> message, Stream outputStream);

        /// <summary>
        /// Creates a new defragmenter stream
        /// </summary>
        /// <returns>Defragmenter stream</returns>
        public abstract IDefragmenterStream CreateDefragmenterStream();
    }
}
