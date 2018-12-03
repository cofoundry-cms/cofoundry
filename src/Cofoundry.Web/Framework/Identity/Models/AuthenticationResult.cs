using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// Represents the result of an authentication request.
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// The user could not be authenticated and errors
        /// have been added to the ModelState.
        /// </summary>
        Failed = 0,

        /// <summary>
        /// The user was authenticated but a password change
        /// is required before the user is able to be logged 
        /// in. The user was not logged in.
        /// </summary>
        PasswordChangeRequired = 1,

        /// <summary>
        /// The user was successfully authenticated and has
        /// been logged in.
        /// </summary>
        Success = 2
    }
}