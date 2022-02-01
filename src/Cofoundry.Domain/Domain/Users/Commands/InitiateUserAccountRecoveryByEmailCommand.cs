using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Initiates an email-based account recovery (AKA "forgot password") request, sending a 
    /// notification to the user with a url to an account recovery form. This command
    /// is designed for self-service password reset so the password is not changed 
    /// until the form has been completed. 
    /// </para>
    /// <para>
    /// Requests are logged and validated to prevent too many account recovery
    /// attempts being initiated in a set period of time.
    /// </para>
    /// </summary>
    public class InitiateUserAccountRecoveryByEmailCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Required. The username of the user account to recover. If the username
        /// is not found on the system the command will complete without error. This 
        /// is to prevent enumeration attacks.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
        /// the user belongs to. This is required because the same username can 
        /// be registered in multiple user areas.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }
    }
}
