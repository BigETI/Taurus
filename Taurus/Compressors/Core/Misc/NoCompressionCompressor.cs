using System;
using System.IO;

namespace Taurus.Compressors
{
    internal class NoCompressionCompressor : ACompressor, INoCompressionCompressor
    {
        public override ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> bytes) => bytes;

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

        public override bool TryCompressingStream(Stream inputStream, Stream outputStream)
        {
            CopyStreamToStream(inputStream, outputStream);
            return true;
        }

        public override bool TryDecompressingStream(Stream inputStream, Stream outputStream)
        {
            CopyStreamToStream(inputStream, outputStream);
            return true;
        }
    }
}
