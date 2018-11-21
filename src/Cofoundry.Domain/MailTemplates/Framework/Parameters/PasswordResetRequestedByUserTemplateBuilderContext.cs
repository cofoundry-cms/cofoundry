using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public class PasswordResetRequestedByUserTemplateBuilderContext
    {
        public UserSummary User { get; set; }

        public Guid UserPasswordResetRequestId { get; set; }

        public string Token { get; set; }
    }
}
