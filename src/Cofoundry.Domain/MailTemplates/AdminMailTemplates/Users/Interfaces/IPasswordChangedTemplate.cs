using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Notifies a user that thier password has been changed.
    /// </summary>
    public interface IPasswordChangedTemplate : IMailTemplate
    {
        string FirstName { get; set; }

        string LastName { get; set; }
    }
}