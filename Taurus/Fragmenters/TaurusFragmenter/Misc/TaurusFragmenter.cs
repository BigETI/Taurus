using System;
using System.IO;
using System.Text;

namespace Taurus.Fragmenters.TaurusFragmenter
{
    internal class TaurusFragmenter : AFragmenter, ITaurusFragmenter
    {
        /// <summary>
        /// Header "tAURUS\r\n"
        /// </summary>
        public static long Header { get; } = 0x0A0D535552554174;

        public override ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> bytes)
        {
            using MemoryStream memory_stream = new MemoryStream();
            Fragment(bytes, memory_stream);
            memory_stream.Seek(0L, SeekOrigin.Begin);
            return memory_stream.ToArray();
        }

        public override void Fragment(ReadOnlySpan<byte> bytes, Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write contents to output stream.", nameof(outputStream));
            }
            using BinaryWriter binary_writer = new BinaryWriter(outputStream, Encoding.UTF8, true);
            binary_writer.Write(Header);
            binary_writer.Write(bytes.Length);
            binary_writer.Write(bytes);
            binary_writer.Flush();
        }

        public override IDefragmenterStream CreateDefragmenterStream() => new TaurusDefragmenterStream();
    }
}
