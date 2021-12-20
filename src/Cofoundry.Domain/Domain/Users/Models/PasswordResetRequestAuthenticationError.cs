namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the different types of errors that can
    /// occur when authenticating a password reset request.
    /// </summary>
    public enum PasswordResetRequestAuthenticationError
    {
        /// <summary>
        /// Default no-error state.
        /// </summary>
        None = 0,

        /// <summary>
        /// Invalid id and token combination. This can include
        /// situations where the id or token are not correctly
        /// formatted.
        /// </summary>
        InvalidRequest = 1,

        /// <summary>
        /// The request exists but has already been completed.
        /// </summary>
        AlreadyComplete = 2,

        /// <summary>
        /// The request exists but has expired.
        /// </summary>
        Expired = 3
    }


}
