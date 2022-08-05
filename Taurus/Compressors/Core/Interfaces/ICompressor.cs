using System;
using System.IO;

namespace Taurus.Compressors
{
    public interface ICompressor
    {
        ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> bytes);
        
        ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> bytes);

        bool TryCompressingStream(Stream inputStream, Stream outputStream);

        bool TryDecompressingStream(Stream inputStream, Stream outputStream);
    }
}
