namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains the result of an authentication test on a password
    /// reset token.
    /// </summary>
    public class PasswordResetRequestAuthenticationResult
    {
        /// <summary>
        /// <see langword="true"/> if the token is valid; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// If the request is invalid, then this will represent the type
        /// of error that has occured.
        /// </summary>
        public PasswordResetRequestAuthenticationError Error { get; set; }
    }
}
