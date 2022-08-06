using System;
using System.IO;

namespace Taurus.Fragmenters
{
    public abstract class AFragmenter : IFragmenter
    {
        public abstract ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> bytes);

        public abstract void Fragment(ReadOnlySpan<byte> bytes, Stream outputStream);

        public abstract IDefragmenterStream CreateDefragmenterStream();
    }
}
