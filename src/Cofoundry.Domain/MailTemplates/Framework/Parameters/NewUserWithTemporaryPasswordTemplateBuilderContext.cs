using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used to format 
    /// a "new user with temporary password" mail template.
    /// </summary>
    public class NewUserWithTemporaryPasswordTemplateBuilderContext
    {
        /// <summary>
        /// The newly created user.
        /// </summary>
        public UserSummary User { get; set; }

        /// <summary>
        /// The temporary password that the user can use to log in to 
        /// the site.
        /// </summary>
        public IHtmlContent TemporaryPassword { get; set; }
    }
}
