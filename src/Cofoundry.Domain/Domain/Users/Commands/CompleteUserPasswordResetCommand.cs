using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
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

        [Required]
        [StringLength(300, MinimumLength = 8)]
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
