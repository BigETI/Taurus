using System;
using System.IO;

namespace Taurus.Serializers
{
    public interface ISerializer
    {
        ReadOnlySpan<byte> Serialize<T>(T obj);

        T Deserialize<T>(ReadOnlySpan<byte> bytes);

        void SerializeToStream<T>(T obj, Stream outputStream);

        T DeserializeFromStream<T>(Stream inputStream);
    }
}
