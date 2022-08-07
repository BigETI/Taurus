using System;
using System.IO;

/// <summary>
/// Taurus serializers namespace
/// </summary>
namespace Taurus.Serializers
{
    /// <summary>
    /// A class that describes a serializer
    /// </summary>
    public abstract class ASerializer : ISerializer
    {
        /// <summary>
        /// Serializes the specified object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Serialized object</returns>
        public virtual ReadOnlySpan<byte> Serialize<T>(T obj)
        {
            using MemoryStream output_memory_stream = new MemoryStream();
            SerializeToStream(obj, output_memory_stream);
            output_memory_stream.Seek(0, SeekOrigin.Begin);
            return output_memory_stream.ToArray();
        }

        /// <summary>
        /// Deserializes the specified bytes into an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="bytes">Bytes</param>
        /// <returns>Deserialized object</returns>
        public virtual T Deserialize<T>(ReadOnlySpan<byte> bytes)
        {
            using MemoryStream input_memory_stream = new MemoryStream();
            input_memory_stream.Write(bytes);
            input_memory_stream.Seek(0, SeekOrigin.Begin);
            return DeserializeFromStream<T>(input_memory_stream);
        }

        /// <summary>
        /// Serializes the specified object into the specified stream
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="outputStream">Output stream</param>
        public abstract void SerializeToStream<T>(T obj, Stream outputStream);

        /// <summary>
        /// Deserializes from the specified stream
        /// </summary>
        /// <typeparam name="T">Deserialized object type</typeparam>
        /// <param name="inputStream">Input stream</param>
        /// <returns>Deserialized object</returns>
        public abstract T DeserializeFromStream<T>(Stream inputStream);
    }
}
