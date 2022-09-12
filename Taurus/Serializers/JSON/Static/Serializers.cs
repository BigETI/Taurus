using Taurus.Serializers.JSON;

namespace Taurus.Serializers
{
    public static partial class Serializers
    {
        public static IJSONSerializer JSONSerializer { get; } = new JSONSerializer();
    }
}
