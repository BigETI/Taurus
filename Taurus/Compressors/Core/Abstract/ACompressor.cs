using System;
using System.IO;

namespace Taurus.Compressors
{
    internal abstract class ACompressor : ICompressor
    {
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

        public abstract bool TryCompressingStream(Stream inputStream, Stream outputStream);

        public abstract bool TryDecompressingStream(Stream inputStream, Stream outputStream);
    }
}
