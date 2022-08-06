using System;
using System.IO;

namespace Taurus.Serializers
{
    public abstract class ASerializer : ISerializer
    {
        public virtual ReadOnlySpan<byte> Serialize<T>(T obj)
        {
            using MemoryStream output_memory_stream = new MemoryStream();
            SerializeToStream(obj, output_memory_stream);
            output_memory_stream.Seek(0, SeekOrigin.Begin);
            return output_memory_stream.ToArray();
        }

        public virtual T Deserialize<T>(ReadOnlySpan<byte> bytes)
        {
            using MemoryStream input_memory_stream = new MemoryStream();
            input_memory_stream.Write(bytes);
            input_memory_stream.Seek(0, SeekOrigin.Begin);
            return DeserializeFromStream<T>(input_memory_stream);
        }

        public abstract void SerializeToStream<T>(T obj, Stream outputStream);

        public abstract T DeserializeFromStream<T>(Stream inputStream);
    }
}
