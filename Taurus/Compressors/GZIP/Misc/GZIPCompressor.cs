using System;
using System.IO;
using System.IO.Compression;

namespace Taurus.Compressors.GZIP
{
    internal class GZIPCompressor : ACompressor, IGZIPCompressor
    {
        public CompressionLevel CompressionLevel { get; }

        public GZIPCompressor(CompressionLevel compressionLevel) => CompressionLevel = compressionLevel;

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
