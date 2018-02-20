using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when a login 
    /// attempt is invalid because too many invalid attempts
    /// have been logged. The requirements for this are configured
    /// in AuthenticationSettings.
    /// </summary>
    public class TooManyFailedAttemptsAuthenticationException : ValidationException
    {
        const string MESSAGE = "Too many failed login attempts have been detected, please try again later.";

        public TooManyFailedAttemptsAuthenticationException()
            : base(MESSAGE)
        {
        }

        public TooManyFailedAttemptsAuthenticationException(string message)
            : base(message)
        {
        }
    }
}
