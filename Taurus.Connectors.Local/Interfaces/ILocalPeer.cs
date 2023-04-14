/// <summary>
/// Taurus connectors local namespace
/// </summary>
namespace Taurus.Connectors.Local
{
    /// <summary>
    /// An interface that represents a local peer
    /// </summary>
    public interface ILocalPeer : IPeer
    {
        /// <summary>
        /// Target local connector
        /// </summary>
        ILocalConnector TargetLocalConnector { get; }
    }
}
