using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <inheritdoc/>
    public class PasswordChangedTemplateBuilderContext : IPasswordChangedTemplateBuilderContext
    {
        public UserSummary User { get; set; }

        public Func<PasswordChangedTemplateBuilderContext, Task<PasswordChangedMailTemplate>> DefaultTemplateFactory { get; set; }

        public Task<PasswordChangedMailTemplate> BuildDefaultTemplateAsync()
        {
            return DefaultTemplateFactory(this);
        }
    }
}