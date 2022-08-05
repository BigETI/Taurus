using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace Taurus.Serializers.BSON
{
    internal class BSONSerializer : ASerializer, IBSONSerializer
    {
        private readonly JsonSerializer bsonSerializer = new JsonSerializer();

        public override T DeserializeFromStream<T>(Stream inputStream)
        {
            using BsonDataReader input_bson_data_reader = new BsonDataReader(inputStream);
            return bsonSerializer.Deserialize<T>(input_bson_data_reader);
        }

        public override void SerializeToStream<T>(T obj, Stream outputStream)
        {
            using BsonDataWriter output_bson_data_writer = new BsonDataWriter(outputStream);
            bsonSerializer.Serialize(output_bson_data_writer, obj);
        }
    }
}
