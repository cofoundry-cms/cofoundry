using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain.MailTemplates.DefaultMailTemplates
{
    /// <summary>
    /// Template for the email sent to a user to notify them that their
    /// password has been changed. This default version of the template 
    /// is used for all user areas except the Cofoundry admin user area.
    /// </summary>
    public class DefaultPasswordChangedMailTemplate : DefaultMailTemplateBase
    {
        public DefaultPasswordChangedMailTemplate()
        {
            SubjectFormat = "{0}: Password changed";
            ViewFile = DefaultMailTemplatePath.TemplateView(nameof(DefaultPasswordChangedMailTemplate));
        }

        /// <summary>
        /// The username of the user who has had their password changed.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The path that the user can use to log in.
        /// </summary>
        public string LoginPath { get; set; }
    }
}