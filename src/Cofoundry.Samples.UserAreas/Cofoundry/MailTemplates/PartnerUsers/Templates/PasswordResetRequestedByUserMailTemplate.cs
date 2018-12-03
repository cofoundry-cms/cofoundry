using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    public class PasswordResetRequestedByUserMailTemplate : IMailTemplate
    {
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(PasswordResetRequestedByUserMailTemplate));

        public string Subject { get; } = "Password reset request";

        public string Username { get; set; }

        public string ResetUrl { get; set; }
    }
}