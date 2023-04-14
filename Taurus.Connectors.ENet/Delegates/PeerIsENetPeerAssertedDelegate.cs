/// <summary>
/// Taurus connectors ENet namespace
/// </summary>
namespace Taurus.Connectors.ENet
{
    /// <summary>
    /// Used to invoke when peer is ENet peer asserted
    /// </summary>
    /// <param name="eNetPeer">ENet peer</param>
    internal delegate void PeerIsENetPeerAssertedDelegate(IENetPeer eNetPeer);
}
