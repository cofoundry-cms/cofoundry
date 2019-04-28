using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent to a new user when their account has
    /// been created with a temporary password. This version of the template 
    /// is used by default for the Cofoundry admin user area.
    /// </summary>
    public class AdminNewUserWithTemporaryPasswordMailTemplate : IMailTemplate
    {
        public AdminNewUserWithTemporaryPasswordMailTemplate()
        {
            ViewFile = AdminMailTemplatePath.TemplateView(nameof(AdminNewUserWithTemporaryPasswordMailTemplate));
            SubjectFormat = "{0}: Your account has been created";
        }

        /// <summary>
        /// Name or full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/MyMailTemplate".
        /// </summary>
        public string ViewFile { get; set; }

        /// <summary>
        /// String to use as the subject to the email. To customize this
        /// use the "SubjectFormat" property.
        /// </summary>
        public string Subject
        {
            get { return string.Format(SubjectFormat, ApplicationName); }
        }

        /// <summary>
        /// String used to format the email subject. This can optionally 
        /// include a token "{0}" which is replaced with the application 
        /// name configuration setting e.g. "{0}: Your account has been created".
        /// </summary>
        public string SubjectFormat { get; set; }

        /// <summary>
        /// The application name to use in formatting the subject. By default
        /// this is retreived from a configuration setting.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The username of the new user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The temporary password that the user can use to log in to 
        /// the site.
        /// </summary>
        public IHtmlContent TemporaryPassword { get; set; }

        /// <summary>
        /// The path that the user can use to log in.
        /// </summary>
        public string LoginPath { get; set; }
    }
}
