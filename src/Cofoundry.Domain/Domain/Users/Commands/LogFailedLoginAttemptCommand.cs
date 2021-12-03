using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    public class LogFailedLoginAttemptCommand : ICommand
    {
        public LogFailedLoginAttemptCommand() { }

        public LogFailedLoginAttemptCommand(string userAreaCode, string username)
        {
            Username = username;
            UserAreaCode = userAreaCode;
        }

        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
        /// attempting to be logged in to.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The username used in the login attempt. This is expected to be in a 
        /// "uniquified" format, as this should have been already processed whenever 
        /// this needs to be called.
        /// </summary>
        [Required]
        public string Username { get; set; }
    }
}
