using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    /// <summary>
    /// Context model containg data that can be used to format 
    /// a "password reset requested by user" mail template.
    /// </summary>
    public class PasswordResetRequestedByUserTemplateBuilderContext
    {
        /// <summary>
        /// The user who is requesting to have their password reset.
        /// </summary>
        public UserSummary User { get; set; }

        /// <summary>
        /// A unique identifier required to authenticate when 
        /// resetting the password.
        /// </summary>
        public Guid UserPasswordResetRequestId { get; set; }

        /// <summary>
        /// A token used to authenticate when resetting the password.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The relative base path used to construct the reset url 
        /// e.g. new Uri("/auth/forgot-password").
        /// </summary>
        public Uri ResetUrlBase { get; set; }
    }
}
