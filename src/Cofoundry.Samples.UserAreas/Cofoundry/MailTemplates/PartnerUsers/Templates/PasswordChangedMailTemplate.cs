using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    /// <summary>
    /// Notifies a user that thier password has been changed.
    /// </summary>
    public class PasswordChangedMailTemplate : IMailTemplate
    {
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(PasswordChangedMailTemplate));

        public string Subject { get; } = "Your Password changed";

        public string Username { get; set; }

        public string LoginPath { get; set; }
    }
}