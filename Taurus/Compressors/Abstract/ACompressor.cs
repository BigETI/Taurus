using System;
using System.IO;

namespace Taurus.Compressors
{
    /// <summary>
    /// An abstract class that describes a compressor
    /// </summary>
    public abstract class ACompressor : ICompressor
    {
        /// <summary>
        /// Compresses the specified bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Compressed bytes</returns>
        public virtual ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> bytes)
        {
            ReadOnlySpan<byte> ret = Array.Empty<byte>();
            using MemoryStream input_memory_stream = new MemoryStream();
            using MemoryStream output_memory_stream = new MemoryStream();
            input_memory_stream.Write(bytes);
            input_memory_stream.Seek(0L, SeekOrigin.Begin);
            if (TryCompressingStream(input_memory_stream, output_memory_stream))
            {
                output_memory_stream.Seek(0L, SeekOrigin.Begin);
                ret = output_memory_stream.ToArray();
            }
            return ret;
        }

        /// <summary>
        /// Decompresses the specified bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Decompressed bytes</returns>
        public virtual ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> bytes)
        {
            ReadOnlySpan<byte> ret = Array.Empty<byte>();
            using MemoryStream input_memory_stream = new MemoryStream();
            using MemoryStream output_memory_stream = new MemoryStream();
            input_memory_stream.Write(bytes);
            input_memory_stream.Seek(0L, SeekOrigin.Begin);
            if (TryDecompressingStream(input_memory_stream, output_memory_stream))
            {
                output_memory_stream.Seek(0L, SeekOrigin.Begin);
                ret = output_memory_stream.ToArray();
            }
            return ret;
        }

        /// <summary>
        /// Tries to compress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        public abstract bool TryCompressingStream(Stream inputStream, Stream outputStream);

        /// <summary>
        /// Tries to decompress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        public abstract bool TryDecompressingStream(Stream inputStream, Stream outputStream);
    }
}
