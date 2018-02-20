using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the domain layer when a user is prevented from
    /// logging in because they have the RequirePasswordChange flag set to true.
    /// This exception should be caught by the GUI layer and handled accordingly 
    /// (i.e. redirected to password change form) before allowing the user to log in.
    /// </summary>
    public class PasswordChangeRequiredException : Exception
    {
        const string MESSAGE = @"The user could not be logged in because they 
are required to change their password first. To support the 'force password change' 
feature you must catch this exception and handle the change password flow within your application.";

        public PasswordChangeRequiredException()
            : base(MESSAGE)
        {
        }

        public PasswordChangeRequiredException(string message)
            : base(message)
        {
        }
    }
}
