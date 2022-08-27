/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An interface that represents an user authentication result
    /// </summary>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">Authentication successful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public interface IUserAuthenticationResult<TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// Is user authentication successful
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        /// Authentication successful message data
        /// </summary>
        public TAuthenticationSuccessfulMessageData? AuthenticationSuccessfulMessageData { get; }

        /// <summary>
        /// Authentication failed message data
        /// </summary>
        public TAuthenticationFailedMessageData? AuthenticationFailedMessageData { get; }
    }
}
