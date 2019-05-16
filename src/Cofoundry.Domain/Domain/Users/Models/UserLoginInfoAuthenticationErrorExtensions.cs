using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class UserLoginInfoAuthenticationErrorExtensions
    {
        /// <summary>
        /// Returns a text a basic text representation of the enum for 
        /// displaying as a user friendly error message. Often you'll want
        /// to customize the error messages produced, but this helper can
        /// be used to cut down on boilerplate code if you are creating
        /// a simple implementation.
        /// </summary>
        public static string ToDisplayText(this UserLoginInfoAuthenticationError error)
        {
            switch (error)
            {
                case UserLoginInfoAuthenticationError.None:
                    return string.Empty;
                case UserLoginInfoAuthenticationError.InvalidCredentials:
                    return "Invalid username or password.";
                case UserLoginInfoAuthenticationError.TooManyFailedAttempts:
                    return "Too many failed login attempts have been detected, please try again later.";
                case UserLoginInfoAuthenticationError.NotSpecified:
                    return "The authentication attempt was unsuccessful.";
                default:
                    throw new Exception($"Unknown {nameof(UserLoginInfoAuthenticationError)} value: {error}");
            }
        }
    }
}
