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
    public class DefaultPasswordResetRequestedByUserMailTemplate : DefaultMailTemplateBase
    {
        public DefaultPasswordResetRequestedByUserMailTemplate()
        {
            SubjectFormat = "{0}: Password reset request";
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultPasswordResetRequestedByUserMailTemplate));
        }

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