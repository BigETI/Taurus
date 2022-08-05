using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Taurus.Serializers.JSON
{
    internal class JSONSerializer : ASerializer, IJSONSerializer
    {
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public override T DeserializeFromStream<T>(Stream inputStream)
        {
            using StreamReader input_stream_reader = new StreamReader(inputStream, Encoding.UTF8, true, 1024, true);
            using JsonTextReader input_json_text_reader = new JsonTextReader(input_stream_reader);
            return jsonSerializer.Deserialize<T>(input_json_text_reader);
        }

        public override void SerializeToStream<T>(T obj, Stream outputStream)
        {
            using StreamWriter output_stream_writer = new StreamWriter(outputStream, Encoding.UTF8, 1024, true);
            jsonSerializer.Serialize(output_stream_writer, obj);
        }
    }
}
