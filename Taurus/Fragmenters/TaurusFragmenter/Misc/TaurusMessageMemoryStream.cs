using System.IO;

namespace Taurus.Fragmenters.TaurusFragmenter
{
    internal class TaurusMessageMemoryStream : MemoryStream
    {
        public uint ExpectedLength { get; }

        public TaurusMessageMemoryStream(uint expectedLength) =>
            ExpectedLength = expectedLength;
    }
}

