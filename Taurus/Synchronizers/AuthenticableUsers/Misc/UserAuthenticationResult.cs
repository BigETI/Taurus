using Taurus.Validators;

/// <summary>
/// Taurus synchronizers authenticable users namespace
/// </summary>
namespace Taurus.Synchronizers.AuthenticableUsers
{
    /// <summary>
    /// A class that describes an user authentication result
    /// </summary>
    /// <typeparam name="TAuthenticatedUserInformation">Authenticated user information</typeparam>
    /// <typeparam name="TAuthenticationSuccessfulMessageData">Authentication successful message data type</typeparam>
    /// <typeparam name="TAuthenticationFailedMessageData">Authentication failed message data type</typeparam>
    public sealed class UserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> :
        IUserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>
        where TAuthenticatedUserInformation : class
        where TAuthenticationSuccessfulMessageData : class, IBaseMessageData
        where TAuthenticationFailedMessageData : class, IBaseMessageData
    {
        /// <summary>
        /// Is user authentication successful
        /// </summary>
        public bool IsSuccessful => AuthenticationSuccessfulMessageData != null;

        /// <summary>
        /// Authenticated user information
        /// </summary>
        public TAuthenticatedUserInformation? AuthenticatedUserInformation { get; }

        /// <summary>
        /// Authentication successful message data
        /// </summary>
        public TAuthenticationSuccessfulMessageData? AuthenticationSuccessfulMessageData { get; }

        /// <summary>
        /// Authentication failed message data
        /// </summary>
        public TAuthenticationFailedMessageData? AuthenticationFailedMessageData { get; }

        /// <summary>
        /// Constructs a new user authentication result
        /// </summary>
        /// <param name="authenticatedUserInformation">Authenticated user information</param>
        /// <param name="authenticationSuccessfulMessageData">Authentication successful message data</param>
        /// <param name="authenticationFailedMessageData">Authentication failed message data</param>
        private UserAuthenticationResult
        (
            TAuthenticatedUserInformation? authenticatedUserInformation,
            TAuthenticationSuccessfulMessageData? authenticationSuccessfulMessageData,
            TAuthenticationFailedMessageData? authenticationFailedMessageData
        )
        {
            AuthenticatedUserInformation = authenticatedUserInformation;
            AuthenticationSuccessfulMessageData = authenticationSuccessfulMessageData;
            AuthenticationFailedMessageData = authenticationFailedMessageData;
        }

        /// <summary>
        /// Creates a new user authentication result from success
        /// </summary>
        /// <param name="authenticationSuccessfulMessageData">Authentication successful message data</param>
        /// <returns>New user authentication result from success</returns>
        public static IUserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> CreateFromSuccess
        (
            TAuthenticatedUserInformation authenticatedUserInformation,
            TAuthenticationSuccessfulMessageData authenticationSuccessfulMessageData
        )
        {
            Validator.Validate(authenticationSuccessfulMessageData, nameof(authenticationSuccessfulMessageData));
            return new UserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>
            (
                authenticatedUserInformation,
                authenticationSuccessfulMessageData,
                null
            );
        }

        /// <summary>
        /// Creates a new user authentication result from failure
        /// </summary>
        /// <param name="authenticationFailedMessageData">Authentication failed message data</param>
        /// <returns>New user authentication result from failure</returns>
        public static IUserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData> CreateFromFailure
        (
            TAuthenticationFailedMessageData authenticationFailedMessageData
        )
        {
            Validator.Validate(authenticationFailedMessageData, nameof(authenticationFailedMessageData));
            return new UserAuthenticationResult<TAuthenticatedUserInformation, TAuthenticationSuccessfulMessageData, TAuthenticationFailedMessageData>
            (
                null,
                null,
                authenticationFailedMessageData
            );
        }
    }
}
