using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents an user
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// User GUID
        /// </summary>
        UserGUID UserGUID { get; }

        /// <summary>
        /// Peer
        /// </summary>
        IPeer Peer { get; }
    }
}
