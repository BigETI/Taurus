using System.Threading.Tasks;

/// <summary>
/// Taurus connectors namespace
/// </summary>
namespace Taurus.Connectors
{
    /// <summary>
    /// Used to handle peer connection attempts
    /// </summary>
    /// <param name="peer">Connecting peer</param>
    /// <returns>"true" if connection should be established, otherwise "false"</returns>
    public delegate Task<bool> HandlePeerConnectionAttemptDelegate(IPeer peer);
}
