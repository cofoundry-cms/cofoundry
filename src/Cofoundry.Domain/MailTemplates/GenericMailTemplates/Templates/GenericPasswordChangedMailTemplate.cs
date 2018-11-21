using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    /// <summary>
    /// Notifies a user that thier password has been changed.
    /// </summary>
    public class GenericPasswordChangedMailTemplate : IMailTemplate
    {
        public GenericPasswordChangedMailTemplate()
        {
            SubjectFormat = "{0}: Password changed";
            ViewFile = GenericMailTemplatePath.TemplateView(nameof(GenericPasswordChangedMailTemplate));
        }

        public string ViewFile { get; set; }

        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        public string SubjectFormat { get; set; }

        public string ApplicationName { get; set; }

        public string Username { get; set; }

        public string LoginPath { get; set; }
    }
}