using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Taurus.Serializers.JSON
{
    /// <summary>
    /// A class that describes a JSON serializer
    /// </summary>
    public sealed class JSONSerializer : ASerializer, IJSONSerializer
    {
        /// <summary>
        /// JSON serializer
        /// </summary>
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        /// <summary>
        /// Deserializes from the specified stream
        /// </summary>
        /// <typeparam name="T">Deserialized object type</typeparam>
        /// <param name="inputStream">Input stream</param>
        /// <returns>Deserialized object</returns>
        public override T DeserializeFromStream<T>(Stream inputStream)
        {
            using StreamReader input_stream_reader = new StreamReader(inputStream, Encoding.UTF8, true, 1024, true);
            using JsonTextReader input_json_text_reader = new JsonTextReader(input_stream_reader);
            return
                jsonSerializer.Deserialize<T>(input_json_text_reader) ??
                    throw new JsonSerializationException("Input stream is not valid JSON.");
        }

        /// <summary>
        /// Serializes object into the specified stream
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="outputStream">Output stream</param>
        public override void SerializeToStream<T>(T obj, Stream outputStream)
        {
            using StreamWriter output_stream_writer = new StreamWriter(outputStream, Encoding.UTF8, 1024, true);
            jsonSerializer.Serialize(output_stream_writer, obj);
        }
    }
}
