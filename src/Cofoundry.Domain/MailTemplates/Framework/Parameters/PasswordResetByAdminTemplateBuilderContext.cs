using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public class PasswordResetByAdminTemplateBuilderContext
    {
        public UserSummary User { get; set; }

        public IHtmlContent TemporaryPassword { get; set; }
    }
}
