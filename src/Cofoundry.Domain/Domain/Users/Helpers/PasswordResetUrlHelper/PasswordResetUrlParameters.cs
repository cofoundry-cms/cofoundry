using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Respresents the parameters required to complete a
    /// password reset that have been parsed from a request 
    /// url. The properties may be null or empty if the request
    /// was invalid.
    /// </summary>
    public class PasswordResetUrlParameters
    {
        /// <summary>
        /// A unique identifier required to authenticate when 
        /// resetting the password. May be Guid.Empty if the
        /// value could not be parsed correctly.
        /// </summary>
        public Guid UserPasswordResetRequestId { get; set; }

        /// <summary>
        /// A token used to authenticate when resetting the password.
        /// May be null if the token was not present in the querystring.
        /// </summary>
        public string Token { get; set; }
    }
}
