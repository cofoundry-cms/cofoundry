using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used to format 
    /// a "password changed" mail template.
    /// </summary>
    public class PasswordChangedTemplateBuilderContext
    {
        /// <summary>
        /// The user that had their password changed.
        /// </summary>
        public UserSummary User { get; set; }
    }
}
