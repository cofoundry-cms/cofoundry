using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Initiates a password reset request, sending a notification
    /// to the user with a url to a password reset form. This command
    /// is designed for self-service password reset so the password
    /// is not changed until the form has been completed. 
    /// </para>
    /// <para>
    /// Requests are logged and validated to prevent too many reset
    /// attempts being initiated in a set period of time.
    /// </para>
    /// </summary>
    public class InitiateUserPasswordResetRequestCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// Required. The username to send the request for. If the username
        /// is not found on the system we don't throw an error. This
        /// is to prevent enumeration attacks.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Users can be registered in multiple
        /// user areas so this is required.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The relative base path used to construct the reset url 
        /// e.g. "/auth/forgot-password".
        /// </summary>
        [Required]
        public string ResetUrlBase { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Uri.IsWellFormedUriString(ResetUrlBase, UriKind.Relative))
            {
                yield return new ValidationResult($"{ResetUrlBase} must be a relative url.", new string[] { nameof(ResetUrlBase) });
            }
        }
    }
}
