using System;
using System.IO;
using System.IO.Compression;

/// <summary>
/// Taurus compressors Gzip namespace
/// </summary>
namespace Taurus.Compressors.GZIP
{
    /// <summary>
    /// A class that describes a Gzip compressor
    /// </summary>
    internal sealed class GZIPCompressor : ACompressor, IGZIPCompressor
    {
        /// <summary>
        /// Compression level
        /// </summary>
        public CompressionLevel CompressionLevel { get; }

        /// <summary>
        /// Constructs a new Gzip compressor
        /// </summary>
        /// <param name="compressionLevel">Compression level</param>
        public GZIPCompressor(CompressionLevel compressionLevel) => CompressionLevel = compressionLevel;

        /// <summary>
        /// Tries to compress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        public override bool TryCompressingStream(Stream inputStream, Stream outputStream)
        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException("Can not read from input stream.", nameof(inputStream));
            }
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write to output stream.", nameof(outputStream));
            }
            bool ret;
            try
            {
                using (GZipStream gzip_stream = new GZipStream(outputStream, CompressionLevel, true))
                {
                    inputStream.CopyTo(gzip_stream);
                }
                ret = true;
            }
            catch (Exception e)
            {
                ret = false;
                Console.Error.WriteLine(e);
            }
            return ret;
        }

        /// <summary>
        /// Tries to decompress the specified stream
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="outputStream">Output stream</param>
        /// <returns>"true" if compression was successful, otherwise "false"</returns>
        public override bool TryDecompressingStream(Stream inputStream, Stream outputStream)
        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException("Can not read from input stream.", nameof(inputStream));
            }
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write to output stream.", nameof(outputStream));
            }
            bool ret;
            try
            {
                using (GZipStream gzip_stream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                {
                    gzip_stream.CopyTo(outputStream);
                }
                ret = true;
            }
            catch (Exception e)
            {
                ret = false;
                Console.Error.WriteLine(e);
            }
            return ret;
        }
    }
}
