using System;
using System.IO;

namespace Taurus.Serializers
{
    /// <summary>
    /// An interface that represents a serializer
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the specified object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Serialized object</returns>
        ReadOnlySpan<byte> Serialize<T>(T obj);

        /// <summary>
        /// Deserializes the specified bytes into an object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="bytes">Bytes</param>
        /// <returns>Deserialized object</returns>
        T Deserialize<T>(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Serializes the specified object into the specified stream
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="outputStream">Output stream</param>
        void SerializeToStream<T>(T obj, Stream outputStream);

        /// <summary>
        /// Deserializes from the specified stream
        /// </summary>
        /// <typeparam name="T">Deserialized object type</typeparam>
        /// <param name="inputStream">Input stream</param>
        /// <returns>Deserialized object</returns>
        T DeserializeFromStream<T>(Stream inputStream);
    }
}
