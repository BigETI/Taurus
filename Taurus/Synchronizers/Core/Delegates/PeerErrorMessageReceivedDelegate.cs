/// <summary>
/// Taurus synchronizers namespace
/// </summary>
namespace Taurus.Synchronizers
{
    /// <summary>
    /// Used to invoke when a per eror message has been received
    /// </summary>
    /// <param name="errorType">Error type</param>
    /// <param name="issuingMessageType">Issuing message type</param>
    /// <param name="message">Error message</param>
    public delegate void PeerErrorMessageReceivedDelegate(EErrorType errorType, string issuingMessageType, string message);
}
