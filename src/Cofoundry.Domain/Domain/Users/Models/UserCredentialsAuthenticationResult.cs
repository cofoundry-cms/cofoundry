using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to wrap a <see cref="UserSignInInfo"/> result with additional
    /// information about why an authentication attempt is unsuccessful.
    /// This is the result of the <see cref="AuthenticateUserCredentialsQuery"/>.
    /// </summary>
    public class UserCredentialsAuthenticationResult : ValidationQueryResult
    {
        /// <summary>
        /// If successful this will be filled with user data; otherwise
        /// it will be null.
        /// </summary>
        public UserSignInInfo User { get; set; }

        /// <summary>
        /// Indicates if the authentication attempt was successful. If
        /// <see langword="true"/> then the User property should have a value.
        /// </summary>
        public override bool IsSuccess { get => base.IsSuccess; set => base.IsSuccess = value; }

        /// <summary>
        /// Indicates the reason if the authentication failed. If authentication succeded 
        /// then this will be <see langword="null"/>.
        /// </summary>
        public override ValidationError Error { get => base.Error; set => base.Error = value; }

        /// <summary>
        /// Creates a new unsuccesful authentication result using the 
        /// <see cref="UserValidationErrors.Authentication.NotSpecified"/> reason.
        /// </summary>
        public static UserCredentialsAuthenticationResult CreateFailedResult()
        {
            return new UserCredentialsAuthenticationResult()
            {
                Error = UserValidationErrors.Authentication.NotSpecified.Create()
            };
        }
    }
}
