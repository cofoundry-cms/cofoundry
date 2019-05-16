using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when a login 
    /// attempt is invalid because the username or password
    /// is invalid.
    /// </summary>
    public class InvalidCredentialsAuthenticationException : PropertyValidationException
    {
        public InvalidCredentialsAuthenticationException(string propertyName)
            : base(UserLoginInfoAuthenticationError.InvalidCredentials.ToDisplayText(), propertyName)
        {
        }

        public InvalidCredentialsAuthenticationException(string propertyName, string message)
            : base(message, propertyName)
        {
        }
    }
}
