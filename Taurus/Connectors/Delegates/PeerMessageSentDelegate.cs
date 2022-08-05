using System;

namespace Taurus.Connectors
{
    public delegate void PeerMessageSentDelegate(IPeer peer, ReadOnlySpan<byte> message);
}
