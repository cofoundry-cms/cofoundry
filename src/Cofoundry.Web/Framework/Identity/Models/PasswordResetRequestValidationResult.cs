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
    public class PasswordResetRequestValidationResult : PasswordResetRequestAuthenticationResult
    {
        /// <summary>
        /// The unique identifier used in the authentication attempt.
        /// </summary>
        public Guid UserPasswordResetRequestId { get; set; }

        /// <summary>
        /// The token used in the authentication attempt.
        /// </summary>
        public string Token { get; set; }
    }
}
