using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.MailTemplates;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    public class CompleteUserPasswordResetCommand : ICommand, ILoggableCommand
    {
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
        /// Template for the notification that tells a user that thier password has been changed.
        /// </summary>
        [Required]
        public IPasswordChangedTemplate MailTemplate { get; set; }
    }
}
