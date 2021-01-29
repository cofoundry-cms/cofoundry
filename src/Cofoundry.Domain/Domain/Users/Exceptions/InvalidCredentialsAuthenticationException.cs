using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when a login 
    /// attempt is invalid because the username or password
    /// is invalid.
    /// </summary>
    public class InvalidCredentialsAuthenticationException : ValidationErrorException
    {
        public InvalidCredentialsAuthenticationException(string propertyName)
            : base(new ValidationError() {
                Message = UserLoginInfoAuthenticationError.InvalidCredentials.ToDisplayText(),
                Properties = new string[] { propertyName }
            })
        {
        }

        public InvalidCredentialsAuthenticationException(string propertyName, string message)
            : base(new ValidationError()
            {
                Message = message,
                Properties = new string[] { propertyName }
            })
        {
        }
    }
}
