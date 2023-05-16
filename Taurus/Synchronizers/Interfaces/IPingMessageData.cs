namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents ping message data
    /// </summary>
    public interface IPingMessageData : IBaseMessageData
    {
        /// <summary>
        /// Key
        /// </summary>
        int? Key { get; set; }
    }
}
