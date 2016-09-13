using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.MailTemplates;

namespace Cofoundry.Domain
{
    public class ResetUserPasswordByUserIdCommand : ICommand, ILoggableCommand, IResetUserPasswordCommand
    {
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

        [Required]
        public IResetPasswordTemplate MailTemplate { get; set; }
    }
}
