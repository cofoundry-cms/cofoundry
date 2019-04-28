using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to resets their 
    /// own password via the 'forgot password' mechanism on partner users
    /// login form.
    /// </summary>
    public class PasswordResetRequestedByUserMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added. E.g. "~/Cofoundry/MailTemplates/MyMailTemplate".
        /// </summary>
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(PasswordResetRequestedByUserMailTemplate));

        /// <summary>
        /// String to use as the subject to the email.
        /// </summary>
        public string Subject { get; } = "Password reset request";

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