﻿using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace Taurus.Serializers.BSON
{
    /// <summary>
    /// A class that describes a BSON serializer
    /// </summary>
    public sealed class BSONSerializer : ASerializer, IBSONSerializer
    {
        /// <summary>
        /// BSON serializer
        /// </summary>
        private readonly JsonSerializer bsonSerializer = new JsonSerializer();

        /// <summary>
        /// Deserializes from the specified stream
        /// </summary>
        /// <typeparam name="T">Deserialized object type</typeparam>
        /// <param name="inputStream">Input stream</param>
        /// <returns>Deserialized object</returns>
        public override T DeserializeFromStream<T>(Stream inputStream)
        {
            using BsonDataReader input_bson_data_reader = new BsonDataReader(inputStream);
            return
                bsonSerializer.Deserialize<T>(input_bson_data_reader) ??
                    throw new JsonSerializationException("Input stream is not valid BSON.");
        }

        /// <summary>
        /// Serializes object into the specified stream
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="outputStream">Output stream</param>
        public override void SerializeToStream<T>(T obj, Stream outputStream) =>
            bsonSerializer.Serialize(new BsonDataWriter(outputStream), obj);
    }
}
