namespace Taurus.Synchronizers
{
    public interface IErrorMessageData
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
