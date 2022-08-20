using Taurus.Connectors;

/// <summary>
/// Taurus synchronizers authenticated users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticatedUsers
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
