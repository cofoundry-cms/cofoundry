using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <inheritdoc/>
    public class AccountVerificationTemplateBuilderContext : IAccountVerificationTemplateBuilderContext
    {
        public UserSummary User { get; set; }

        public string Token { get; set; }

        public string VerificationUrlPath { get; set; }

        public Func<AccountVerificationTemplateBuilderContext, Task<AccountVerificationMailTemplate>> DefaultTemplateFactory { get; set; }

        public Task<AccountVerificationMailTemplate> BuildDefaultTemplateAsync()
        {
            return DefaultTemplateFactory(this);
        }
    }
}