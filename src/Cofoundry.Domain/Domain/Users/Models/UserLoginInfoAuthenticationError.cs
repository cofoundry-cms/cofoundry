using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to indicate the type of error that
    /// prevented a user from logging in.
    /// </summary>
    public enum UserLoginInfoAuthenticationError
    {
        /// <summary>
        /// Default value or no error set.
        /// </summary>
        None,

        /// <summary>
        /// The user credentials are invalid.
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// Too many failed login attempts have occured either for the
        /// username or IP address.
        /// </summary>
        TooManyFailedAttempts,

        /// <summary>
        /// The error was not specified. This can be used when an error
        /// is picked up outside of the core login function occurs e.g.
        /// in MVC if the ModelState is invalid and the result is returned
        /// before authentication is attempted.
        /// </summary>
        NotSpecified
    }
}
