using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used
    /// to format a "password reset by admin" mail template
    /// </summary>
    public class PasswordResetByAdminTemplateBuilderContext
    {
        /// <summary>
        /// The user that has had their password reset.
        /// </summary>
        public UserSummary User { get; set; }

        /// <summary>
        /// The temporary password that the user can use to log in to 
        /// the site.
        /// </summary>
        public IHtmlContent TemporaryPassword { get; set; }
    }
}
