using System;
using System.IO;

namespace Taurus.Fragmenters
{
    internal class NoFragmentationFragmenter : AFragmenter, INoFragmentationFragmenter
    {
        public override ReadOnlySpan<byte> Fragment(ReadOnlySpan<byte> bytes) => bytes;

        public override void Fragment(ReadOnlySpan<byte> bytes, Stream outputStream)
        {
            if (!outputStream.CanWrite)
            {
                throw new ArgumentException("Can not write contents to output stream.", nameof(outputStream));
            }
            outputStream.Write(bytes);
        }

        public override IDefragmenterStream CreateDefragmenterStream() => new NoDefragmenterStream();
    }
}
