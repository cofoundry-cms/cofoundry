using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to wrap a UserLoginInfo result with additional
    /// information about why an authentication attempt is
    /// unsuccessful.
    /// </summary>
    public class UserLoginInfoAuthenticationResult
    {
        /// <summary>
        /// Indicates the reason if the authentication
        /// failed.
        /// </summary>
        public UserLoginInfoAuthenticationError Error { get; set; }

        /// <summary>
        /// If successful this will be filled with user data; otherwise
        /// it will be null.
        /// </summary>
        public UserLoginInfo User { get; set; }

        /// <summary>
        /// Indicates if the authentication attempt was successful. If
        /// true then the User property should have a value.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Throws an exception if the authentication attempt was 
        /// unsuccessful, using a ValidationException representing
        /// the correct error type.
        /// </summary>
        /// <param name="passwordPropertyName">
        /// The name of the password property, which is used in creating
        /// an InvalidCredentialsAuthenticationException if the credentials 
        /// are invalid.
        /// </param>
        public void ThrowIfUnsuccessful(string passwordPropertyName)
        {
            if (!IsSuccess)
            {
                switch (Error)
                {
                    case UserLoginInfoAuthenticationError.InvalidCredentials:
                        throw new InvalidCredentialsAuthenticationException(passwordPropertyName);
                    case UserLoginInfoAuthenticationError.TooManyFailedAttempts:
                        throw new TooManyFailedAttemptsAuthenticationException();
                    case UserLoginInfoAuthenticationError.NotSpecified:
                        throw new ValidationException(UserLoginInfoAuthenticationError.NotSpecified.ToDisplayText());
                    default:
                        throw new InvalidOperationException($"Unexpected {nameof(UserLoginInfoAuthenticationError)} value '{Error}'.");
                }
            }
        }

        /// <summary>
        /// Creates a new unsuccesful authentication result, optionally
        /// with a specific reason.
        /// </summary>
        /// <returns>The reason for the auth failure. Defaults to UserLoginInfoAuthenticationError.NotSpecified</returns>
        public static UserLoginInfoAuthenticationResult CreateFailedResult(UserLoginInfoAuthenticationError reason = UserLoginInfoAuthenticationError.NotSpecified)
        {
            return new UserLoginInfoAuthenticationResult()
            {
                Error = reason
            };
        }
    }
}
