using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    public class GenericPasswordResetRequestedByUserMailTemplate : IMailTemplate
    {
        public GenericPasswordResetRequestedByUserMailTemplate()
        {
            SubjectFormat = "{0}: Password reset request";
            ViewFile = GenericMailTemplatePath.TemplateView(nameof(GenericPasswordResetRequestedByUserMailTemplate));
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