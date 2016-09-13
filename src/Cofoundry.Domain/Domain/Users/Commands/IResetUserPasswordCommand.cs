using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.MailTemplates;

namespace Cofoundry.Domain
{
    public interface IResetUserPasswordCommand
    {
        IResetPasswordTemplate MailTemplate { get; set; }
    }
}
