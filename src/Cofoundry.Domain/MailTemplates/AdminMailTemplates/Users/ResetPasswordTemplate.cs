using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates
{
    public class ResetPasswordTemplate : IResetPasswordTemplate
    {
        public ResetPasswordTemplate()
        {
            SubjectFormat = "{0}: Password reset request";
            ViewFile = TemplatePath.ViewPath + "Users/ResetPassword";
        }

        public string ViewFile { get; set; }

        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        public string SubjectFormat { get; set; }

        public string ApplicationName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid UserPasswordResetRequestId { get; set; }

        public string Token { get; set; }
    }
}