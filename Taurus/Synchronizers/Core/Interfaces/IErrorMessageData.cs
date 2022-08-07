/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// An interface that reprsents error message data
    /// </summary>
    public interface IErrorMessageData : IBaseMessageData
    {
        /// <summary>
        /// Error type
        /// </summary>
        EErrorType ErrorType { get; set; }

        /// <summary>
        /// Message type that caused this error
        /// </summary>
        public string? IssuingMessageType { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        string? Message { get; set; }
    }
}
