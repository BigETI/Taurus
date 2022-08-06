using System;
using System.IO;

namespace Taurus.Fragmenters
{
    public interface IFragmenter
    {
        ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> bytes);

        void Fragment(ReadOnlySpan<byte> bytes, Stream outputStream);

        IDefragmenterStream CreateDefragmenterStream();
    }
}
