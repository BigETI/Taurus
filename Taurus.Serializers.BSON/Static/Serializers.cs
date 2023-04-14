using Taurus.Serializers.BSON;

namespace Taurus.Serializers
{
    public static partial class Serializers
    {
        public static IBSONSerializer BSONSerializer { get; } = new BSONSerializer();
    }
}
