using System;
using System.IO;

/// <summary>
/// Taurus compressors namespace
/// </summary>
namespace Taurus.Compressors
{
    /// <summary>
    /// A class that describes a no compression compressor
    /// </summary>
    public sealed class NoCompressionCompressor : ACompressor, INoCompressionCompressor
    {
        /// <summary>
        /// Copies the specified input stream to the specified output stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <exception cref="ArgumentException">When "inputStream" can not be read from or "outputStream" can not be written to</exception>
        private void CopyStreamToStream(Stream inputStream, Stream outputStream)
        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException("Can not read from input stream.", nameof(inputStream));
            }
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write to output stream.", nameof(outputStream));
            }
            inputStream.CopyTo(outputStream);
        }

        /// <summary>
        /// Compresses the specified bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Compressed bytes</returns>
        public override ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> bytes) => bytes;

        /// <summary>
        /// Decompresses the specified bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Decompressed bytes</returns>
        public override ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> bytes) => bytes;

        /// <summary>
        /// Tries to compress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        public override bool TryCompressingStream(Stream inputStream, Stream outputStream)
        {
            CopyStreamToStream(inputStream, outputStream);
            return true;
        }

        /// <summary>
        /// Tries to decompress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        public override bool TryDecompressingStream(Stream inputStream, Stream outputStream)
        {
            CopyStreamToStream(inputStream, outputStream);
            return true;
        }
    }
}
