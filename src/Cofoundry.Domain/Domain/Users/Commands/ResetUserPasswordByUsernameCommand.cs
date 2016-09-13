using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.MailTemplates;

namespace Cofoundry.Domain
{
    public class ResetUserPasswordByUsernameCommand : ICommand, ILoggableCommand, IResetUserPasswordCommand
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string UserAreaCode { get; set; }

        [Required]
        public IResetPasswordTemplate MailTemplate { get; set; }
    }
}
