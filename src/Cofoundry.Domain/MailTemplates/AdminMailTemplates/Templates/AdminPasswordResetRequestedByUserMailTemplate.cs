using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    public class AdminPasswordResetRequestedByUserMailTemplate : IMailTemplate
    {
        public AdminPasswordResetRequestedByUserMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminPasswordResetRequestedByUserMailTemplate));
            SubjectFormat = "{0}: Password reset request";
        }

        public string ViewFile { get; set; }

        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        public string SubjectFormat { get; set; }

        public string ApplicationName { get; set; }

        public string Username { get; set; }

        public string ResetUrl { get; set; }
    }
}