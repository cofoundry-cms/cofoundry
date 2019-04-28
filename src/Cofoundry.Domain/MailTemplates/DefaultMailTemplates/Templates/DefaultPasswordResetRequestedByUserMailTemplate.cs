using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to resets their 
    /// own password which is usually via a 'forgot password' mechanism on
    /// a login form. This default version of the template is used for all 
    /// user areas except the Cofoundry admin user area.
    /// </summary>
    public class DefaultPasswordResetRequestedByUserMailTemplate : IMailTemplate
    {
        public DefaultPasswordResetRequestedByUserMailTemplate()
        {
            SubjectFormat = "{0}: Password reset request";
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultPasswordResetRequestedByUserMailTemplate));
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
        /// name configuration setting e.g. "{0}: Password reset request".
        /// </summary>
        public string SubjectFormat { get; set; }

        /// <summary>
        /// The application name to use in formatting the subject. By default
        /// this is retreived from a configuration setting.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The username of the user who is requesting to have their 
        /// password reset.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The url that the user should follow to complete the password reset.
        /// </summary>
        public string ResetUrl { get; set; }
    }
}