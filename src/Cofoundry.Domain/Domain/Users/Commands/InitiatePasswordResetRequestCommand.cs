using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

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
    public class InitiatePasswordResetRequestCommand : ICommand, ILoggableCommand
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

        [Required]
        /// <summary>
        /// The relative base path used to construct the reset url 
        /// e.g. new Uri("/auth/forgot-password").
        /// </summary>
        public Uri ResetUrlBase { get; set; }
    }
}
