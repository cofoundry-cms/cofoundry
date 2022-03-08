using Microsoft.AspNetCore.Html;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <inheritdoc/>
    public class PasswordResetTemplateBuilderContext : IPasswordResetTemplateBuilderContext
    {
        public UserSummary User { get; set; }

        public IHtmlContent TemporaryPassword { get; set; }

        public Func<PasswordResetTemplateBuilderContext, Task<PasswordResetMailTemplate>> DefaultTemplateFactory { get; set; }

        public Task<PasswordResetMailTemplate> BuildDefaultTemplateAsync()
        {
            return DefaultTemplateFactory(this);
        }
    }
}