using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.GenericMailTemplates
{
    /// <summary>
    /// Template for the email sent when an administrator resets a users 
    /// password either programatically or via the admin panel. This generic
    /// version of the template is used by default for all user areas except
    /// the Cofoundry admin user area.
    /// </summary>
    public class GenericPasswordResetByAdminMailTemplate : IMailTemplate
    {
        public GenericPasswordResetByAdminMailTemplate()
        {
            ViewFile = GenericMailTemplatePath.TemplateView(nameof(GenericPasswordResetByAdminMailTemplate));
            SubjectFormat = "{0}: Your password has been reset";
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
        /// name configuration setting e.g. "{0}: Your password has been reset".
        /// </summary>
        public string SubjectFormat { get; set; }

        /// <summary>
        /// The application name to use in formatting the subject. By default
        /// this is retreived from a configuration setting.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The username of the user who has had their password reset.
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
