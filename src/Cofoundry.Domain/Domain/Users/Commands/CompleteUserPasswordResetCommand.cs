using Cofoundry.Domain.CQS;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    public class CompleteUserPasswordResetCommand : ICommand, ILoggableCommand
    {
        public CompleteUserPasswordResetCommand()
        {
            SendNotification = true;
        }

        [Required]
        public Guid UserPasswordResetRequestId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The value to set as the new account password. The password will go through additional validation depending 
        /// on the password policy configuration.
        /// </summary>
        [Required]
        [StringLength(PasswordOptions.MAX_LENGTH_BOUNDARY, MinimumLength = PasswordOptions.MIN_LENGTH_BOUNDARY)]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }

        /// <summary>
        /// Indicates whether to send a notification to the user to let them
        /// know their password has been changed. Defaults to true.
        /// </summary>
        public bool SendNotification { get; set; }
    }
}
