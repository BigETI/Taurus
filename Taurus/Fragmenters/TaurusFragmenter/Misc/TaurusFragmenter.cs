using System;
using System.IO;
using System.Text;

/// <summary>
/// Taurus fragmenters Taurus fragmenter namespace
/// </summary>
namespace Taurus.Fragmenters.TaurusFragmenter
{
    /// <summary>
    /// A class that describes a Taurus fragmenter
    /// </summary>
    internal sealed class TaurusFragmenter : AFragmenter, ITaurusFragmenter
    {
        /// <summary>
        /// Header "tAURUS\r\n"
        /// </summary>
        public static long Header { get; } = 0x0A0D535552554174;

        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Fragmented message</returns>
        public override ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> message)
        {
            using MemoryStream memory_stream = new MemoryStream();
            Fragment(message, memory_stream);
            memory_stream.Seek(0L, SeekOrigin.Begin);
            return memory_stream.ToArray();
        }

        /// <summary>
        /// Fragments the specified message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="outputStream">Output stream</param>
        public override void Fragment(ReadOnlySpan<byte> message, Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write contents to output stream.", nameof(outputStream));
            }
            using BinaryWriter binary_writer = new BinaryWriter(outputStream, Encoding.UTF8, true);
            binary_writer.Write(Header);
            binary_writer.Write(message.Length);
            binary_writer.Write(message);
            binary_writer.Flush();
        }

        /// <summary>
        /// Creates a new defragmenter stream
        /// </summary>
        /// <returns>Defragmenter stream</returns>
        public override IDefragmenterStream CreateDefragmenterStream() => new TaurusDefragmenterStream();
    }
}
