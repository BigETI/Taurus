namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// An interface that represents an user authentication result
    /// </summary>
    /// <typeparam name="TAuthenticatedUserInformation">AUthenticated user information type</typeparam>
    /// <typeparam name="TAuthenticationFailReason">AUthentication fail reason type</typeparam>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">Authentication successful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public interface IUserAuthenticationResult
    <
        TAuthenticatedUserInformation,
        TAuthenticationFailReason,
        TAuthenticationSuccessfulMessageData,
        TAuthenticationFailedMessageData
    >
        where TAuthenticatedUserInformation : class
        where TAuthenticationFailReason : class
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// Is user authentication successful
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        /// Authenticated user information
        /// </summary>
        TAuthenticatedUserInformation? AuthenticatedUserInformation { get; }

        /// <summary>
        /// Authentication fail reason
        /// </summary>
        TAuthenticationFailReason? AuthenticationFailReason { get; }

        /// <summary>
        /// Authentication successful message data
        /// </summary>
        TAuthenticationSuccessfulMessageData? AuthenticationSuccessfulMessageData { get; }

        /// <summary>
        /// Authentication failed message data
        /// </summary>
        TAuthenticationFailedMessageData? AuthenticationFailedMessageData { get; }
    }
}
