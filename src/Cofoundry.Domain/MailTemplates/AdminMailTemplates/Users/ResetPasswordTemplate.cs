using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cofoundry.Domain.MailTemplates
{
    public class ResetPasswordTemplate : IResetPasswordTemplate
    {
        public ResetPasswordTemplate()
        {
            Subject = "Cofoundry: Your new password";
            ViewFile = TemplatePath.ViewPath + "Users/ResetPassword";
        }

        public string ViewFile { get; set; }

        public string Subject { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid UserPasswordResetRequestId { get; set; }

        public string Token { get; set; }
    }
}