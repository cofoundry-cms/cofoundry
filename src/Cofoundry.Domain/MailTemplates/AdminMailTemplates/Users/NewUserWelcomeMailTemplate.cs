using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates
{
    public class NewUserWelcomeMailTemplate : IMailTemplate
    {
        public NewUserWelcomeMailTemplate()
        {
            ViewFile = TemplatePath.ViewPath + "Users/NewUserWelcomeMail";
            SubjectFormat = "{0}: Your account has been created";
        }

        public string ViewFile { get; set; }

        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        #region custom properties

        public string SubjectFormat { get; set; }

        public string ApplicationName { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public HtmlString TemporaryPassword { get; set; }

        #endregion
    }
}
