using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    public class PasswordResetByAdminMailTemplate : IMailTemplate
    {
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(PasswordResetByAdminMailTemplate));

        public string Subject { get; } = "Your password has been reset";

        public string Username { get; set; }

        public IHtmlContent TemporaryPassword { get; set; }

        public string LoginPath { get; set; }
    }
}
