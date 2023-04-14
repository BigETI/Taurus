/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents pong message data
    /// </summary>
    public interface IPongMessageData : IBaseMessageData
    {
        /// <summary>
        /// Key
        /// </summary>
        int? Key { get; set; }
    }
}
