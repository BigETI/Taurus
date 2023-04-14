using System;
using System.IO;

/// <summary>
/// Taurus compressors namespace
/// </summary>
namespace Taurus.Compressors
{
    /// <summary>
    /// An interace that represents a compressor
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compresses the specified bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Compressed bytes</returns>
        ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Decompresses the specified bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Decompressed bytes</returns>
        ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Tries to compress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        bool TryCompressingStream(Stream inputStream, Stream outputStream);

        /// <summary>
        /// Tries to decompress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        bool TryDecompressingStream(Stream inputStream, Stream outputStream);
    }
}
