namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that represents the base of any sent or received data
    /// </summary>
    public interface IBaseMessageData : IValidable
    {
        /// <summary>
        /// Message type
        /// </summary>
        string? MessageType { get; }
    }
}
