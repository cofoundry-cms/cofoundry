using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    public class GenericPasswordResetByAdminMailTemplate : IMailTemplate
    {
        public GenericPasswordResetByAdminMailTemplate()
        {
            ViewFile = GenericMailTemplatePath.TemplateView(nameof(GenericPasswordResetByAdminMailTemplate));
            SubjectFormat = "{0}: Your password has been reset";
        }

        public string ViewFile { get; set; }

        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        #region custom properties

        public string SubjectFormat { get; set; }

        public string ApplicationName { get; set; }

        public string Username { get; set; }

        public IHtmlContent TemporaryPassword { get; set; }

        public string LoginPath { get; set; }

        #endregion
    }
}
