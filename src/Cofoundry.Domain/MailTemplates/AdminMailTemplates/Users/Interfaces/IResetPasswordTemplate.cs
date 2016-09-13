using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cofoundry.Domain.MailTemplates
{
    public interface IResetPasswordTemplate : IMailTemplate
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        Guid UserPasswordResetRequestId { get; set; }

        string Token { get; set; }
    }
}