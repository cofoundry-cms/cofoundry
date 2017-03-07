using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains the result of an authentication test on a password
    /// reset token.
    /// </summary>
    public class PasswordResetRequestAuthenticationResult
    {
        /// <summary>
        /// True if the token is valid; otherwise false.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Contains user a friendly error message if the request is not 
        /// valid. This message is safe to display to a user.
        /// </summary>
        public string ValidationErrorMessage { get; set; }
    }
}
