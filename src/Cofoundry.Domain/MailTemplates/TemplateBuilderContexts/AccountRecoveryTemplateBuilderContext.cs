namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class AccountRecoveryTemplateBuilderContext : IAccountRecoveryTemplateBuilderContext
{
    public required UserSummary User { get; set; }

    public required string Token { get; set; }

    public string? RecoveryUrlPath { get; set; } = string.Empty;

    public required Func<AccountRecoveryTemplateBuilderContext, Task<AccountRecoveryMailTemplate>> DefaultTemplateFactory { get; set; }

    public Task<AccountRecoveryMailTemplate> BuildDefaultTemplateAsync()
    {
        return DefaultTemplateFactory(this);
    }
}
