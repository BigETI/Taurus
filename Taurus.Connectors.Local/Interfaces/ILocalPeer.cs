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
        /// Target local peer
        /// </summary>
        ILocalPeer? TargetLocalPeer { get; internal set; }
    }
}
