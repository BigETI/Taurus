using System.IO.Compression;

namespace Taurus.Compressors.GZIP
{
    public interface IGZIPCompressor : ICompressor
    {
        CompressionLevel CompressionLevel { get; }
    }
}
