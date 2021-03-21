using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates.AdminMailTemplates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.Mail.AdminMailTemplates
{
    /// <summary>
    /// Template for the email sent when a user requests to resets their 
    /// own password via the 'forgot password' mechanism on login form. 
    /// This version of the template is used by default for the Cofoundry
    /// admin user area.
    /// </summary>
    public class ExampleAdminPasswordResetRequestedByUserMailTemplate : IMailTemplate
    {
        /// <summary>
        /// Name or full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this is automatically 
        /// added.
        /// </summary>
        public string ViewFile { get; set; } = "~/Cofoundry/Admin/MailTemplates/Templates/ExampleAdminPasswordResetRequestedByUserMailTemplate";

        /// <summary>
        /// String to use as the subject to the email. To customize this
        /// use the "SubjectFormat" property.
        /// </summary>
        public string Subject { get; } = "We've received a request to reset your password!";

        /// <summary>
        /// In our custom template we are including the first name field which 
        /// is not included in the standard Cofoundry template.
        /// </summary>
        public string FirstName { get; set; }

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