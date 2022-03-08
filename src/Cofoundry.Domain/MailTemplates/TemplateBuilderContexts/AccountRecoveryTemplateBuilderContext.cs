using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <inheritdoc/>
    public class AccountRecoveryTemplateBuilderContext : IAccountRecoveryTemplateBuilderContext
    {
        public UserSummary User { get; set; }

        public string Token { get; set; }

        public string RecoveryUrlPath { get; set; }

        public Func<AccountRecoveryTemplateBuilderContext, Task<AccountRecoveryMailTemplate>> DefaultTemplateFactory { get; set; }

        public Task<AccountRecoveryMailTemplate> BuildDefaultTemplateAsync()
        {
            return DefaultTemplateFactory(this);
        }
    }
}