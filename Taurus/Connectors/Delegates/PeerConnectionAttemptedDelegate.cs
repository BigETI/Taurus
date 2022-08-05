namespace Taurus.Connectors
{
    /// <summary>
    /// Used to signal a peer connection attempt
    /// </summary>
    /// <param name="peer">Connecting peer</param>
    public delegate void PeerConnectionAttemptedDelegate(IPeer peer);
}
