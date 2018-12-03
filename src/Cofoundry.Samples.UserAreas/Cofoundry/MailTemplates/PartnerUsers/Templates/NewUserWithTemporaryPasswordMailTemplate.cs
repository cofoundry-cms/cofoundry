using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Samples.UserAreas.PartnerMailTemplates
{
    public class NewUserWithTemporaryPasswordMailTemplate : IMailTemplate
    {
        public string ViewFile { get; } = PartnerMailTemplatePath.TemplateView(nameof(NewUserWithTemporaryPasswordMailTemplate));

        public string Subject { get; } = "Your account has been created";

        public string Username { get; set; }

        public IHtmlContent TemporaryPassword { get; set; }

        public string LoginPath { get; set; }
    }
}
