using System;
using System.IO;

namespace Taurus.Fragmenters
{
    public interface IDefragmenterStream : IStream
    {
        bool IsMessagePending { get; }

        uint AvailableMessageCount { get; }

        bool TryDequeuingMessage(out ReadOnlySpan<byte> message);

        bool TryDequeuingMessage(Stream outputStream);
    }
}
