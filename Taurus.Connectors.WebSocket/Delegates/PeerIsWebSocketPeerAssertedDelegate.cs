namespace Taurus.Connectors.WebSocket
{
    /// <summary>
    /// Used to invoke when a peer is a WebSocket peer asserted
    /// </summary>
    /// <param name="webSocketPeer">WebSocket peer</param>
    internal delegate void PeerIsWebSocketPeerAssertedDelegate(IWebSocketPeer webSocketPeer);
}
